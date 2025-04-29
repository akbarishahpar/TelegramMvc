using Microsoft.AspNetCore.Http.Features;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramMvc.Contracts;
using TelegramMvc.Enums;
using TelegramMvc.Extensions;
using TelegramMvc.Factories;
using TelegramMvc.Models.Settings;

namespace TelegramMvc.Middleware
{
    public class TelegramMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimiter _rateLimiter;
        private readonly Encoder _encoder;
        private readonly BotSettings _botSettings;
        private readonly ILogger<TelegramMiddleware> _logger;
        private readonly TelegramBotClient _telegramBotClient;

        public TelegramMiddleware(RequestDelegate next, RateLimiter rateLimiter, Encoder encoder,
            BotSettings botSettings, ILogger<TelegramMiddleware> logger)
        {
            _next = next;
            _rateLimiter = rateLimiter;
            _encoder = encoder;
            _botSettings = botSettings;
            _logger = logger;
            _telegramBotClient = TelegramBotClientFactory.Create(botSettings);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Checking if the request path is a mapped to webhook address
            if (!context.Request.IsWebHookRequest(_botSettings.Token))
            {
                await _next(context);
                return;
            }

            //Converting http request body to telegram update
            var update = await context.Request.ToTelegramUpdateAsync(context.RequestAborted);

            try
            {
                using var scope = context.RequestServices.CreateScope();

                // Prevent middleware from processing not supported update types
                if (!update.IsTypeSupported())
                {
                    _logger.LogWarning(
                        "Rejected telegram-bot request with id `{@traceIdentifier}` due to unsupported type.",
                        context.TraceIdentifier);
                    return;
                }

                // Limiting requests per chat id according to the bot settings
                if (_rateLimiter.ShouldLimit(
                        update.GetChatId(),
                        _botSettings.RateLimitSettings?.DelayBetweenRequestsInSeconds))
                {
                    await DisplayRateLimitMessageAsync(update);

                    _logger.LogWarning("Banned telegram-bot request with id `{@traceIdentifier}` due to rate limit.",
                        context.TraceIdentifier);

                    return;
                }

                _logger.LogInformation("Accepted telegram-bot request with id `{@traceIdentifier}`.",
                    context.TraceIdentifier);

                var chatAccessor = scope.ServiceProvider.GetRequiredService<IChatAccessor>();
                var messageWriter = scope.ServiceProvider.GetService<IMessageWriter>();

                await chatAccessor.SetProfileAsync(update.GetChat(), context.RequestAborted);

                //Extracting text message from the update
                var textMessage = update.GetTextMessage().EmptyIfNull();

                //Sending received message to the messageWriter which is implemented by the client of the framework
                if (messageWriter is not null)
                {
                    await messageWriter.WriteAsync(update.GetChatId(), textMessage, MessageDirection.In,
                        context.RequestAborted);
                }

                // If the user is not using InlineKeyboard then the text is sent directly
                if (update.CallbackQuery?.Data == null)
                {
                    // Removing old sent message keyboard from the bot when user types text
                    var previousMessageId =
                        await chatAccessor.GetPreviousMessageIdAsync(update.GetChatId(), context.RequestAborted);

                    if (previousMessageId != null)
                    {
                        await _telegramBotClient.TryDeleteHistoryAsync(update.GetChatId(), previousMessageId,
                            _botSettings.ChatHistoryLevel);
                        await chatAccessor.ForgetPreviousMessageIdAsync(update.GetChatId(),
                            context.RequestAborted);
                    }

                    // Using user current path (There is no CallBackData to replace path of user with that)
                    if (!textMessage.IsUrl())
                        context.Request.SetPath(
                            await chatAccessor.GetPathAsync(update.GetChatId(), context.RequestAborted)
                        );
                }

                while (!context.RequestAborted.IsCancellationRequested)
                {
                    // Routing user to the CallBackData path (* everything which is started with `/` is treated like a url)
                    if (textMessage.IsUrl())
                    {
                        var path = _encoder.Pop(textMessage);
                        if (!string.IsNullOrEmpty(_botSettings.AreaName) &&
                            !path.ToLower().StartsWith(_botSettings.AreaName))
                            path = _botSettings.AreaName + path;
                        await chatAccessor.SetPathAsync(update.GetChatId(), path, context.RequestAborted);
                        context.Request.SetPath(path);
                    }

                    var endpointFeature = context.Features.Get<IEndpointFeature>();
                    if (endpointFeature is not null)
                        endpointFeature.Endpoint = null;

                    await _next(context);

                    if (context.Response.StatusCode != (int)HttpStatusCode.OK)
                        throw new Exception($"Received not ok response ({context.Response.StatusCode}).");

                    if (context.Items.ContainsKey("telegram-redirect"))
                    {
                        textMessage = (context.Items["telegram-redirect"]?.ToString()).EmptyIfNull();
                        context.Items.Remove("telegram-redirect");
                        context.Response.Clear();
                    }
                    else
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "An error occurred while processing telegram-bot request with id `{@traceIdentifier}`.",
                    context.TraceIdentifier);

                await _telegramBotClient.SendMessage(update.GetChatId(),
                    $"An error occurred while processing telegram-bot request with id `{context.TraceIdentifier}`.",
                    cancellationToken: context.RequestAborted);
            }
            finally
            {
                _rateLimiter.UpdateLastAccessTime(update.GetChatId());
                context.Response.StatusCode = 200;
            }
        }

        private async Task DisplayRateLimitMessageAsync(Update update)
        {
            if (update.CallbackQuery?.Data == null || string.IsNullOrEmpty(_botSettings.RateLimitSettings?.Message))
                return;

            await _telegramBotClient.AnswerCallbackQuery(
                update.CallbackQuery.Id,
                _botSettings.RateLimitSettings!.Message,
                cacheTime: (int)_botSettings.RateLimitSettings.DelayBetweenRequestsInSeconds
            );
        }
    }
}