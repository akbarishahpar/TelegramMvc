using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Telegram.Bot;
using TelegramMvc.Contracts;
using TelegramMvc.Enums;
using TelegramMvc.Extensions;
using TelegramMvc.Factories;
using TelegramMvc.Models.Views;

namespace TelegramMvc.Filters;

public class TelegramResultFilterAttribute : ActionFilterAttribute
{
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is TelegramView telegramView)
        {
            var chatAccessor = context.HttpContext.RequestServices.GetRequiredService<IChatAccessor>();
            var messageWriter = context.HttpContext.RequestServices.GetService<IMessageWriter>();
            var telegramBotClient = TelegramBotClientFactory.Create(telegramView.BotSettings);
            
            foreach (var view in telegramView.Results)
            {
                await ProcessViewAsync(
                    context,
                    telegramView,
                    chatAccessor,
                    messageWriter,
                    view,
                    telegramBotClient,
                    context.HttpContext.RequestAborted
                );
            }
        }
    }

    private static async Task ProcessViewAsync(
        ResultExecutingContext context,
        TelegramView telegramView,
        IChatAccessor chatAccessor,
        IMessageWriter? messageWriter,
        TelegramResult view,
        ITelegramBotClient telegramBotClient,
        CancellationToken cancellationToken
    )
    {
        var chatHistoryLevel = telegramView.BotSettings.ChatHistoryLevel;

        var chatId = telegramView.Update.GetChatId();

        var previousMessageId = await chatAccessor.GetPreviousMessageIdAsync(chatId,
            cancellationToken);

        if (view is TelegramRedirect telegramRedirect)
        {
            context.HttpContext.Items["telegram-redirect"] = telegramRedirect.Path;
            context.Cancel = true;
        }
        else if (view is TelegramVoid telegramVoid)
        {
            if (telegramVoid.TryDeleteHistory)
                await telegramBotClient.TryDeleteHistoryAsync(chatId, previousMessageId, chatHistoryLevel);

            context.Result = new OkResult();
        }
        else
        {
            var sentMessage = view switch
            {
                TelegramMessage message => await telegramBotClient.SendTextMessageAsync(chatId, message,
                    previousMessageId, chatHistoryLevel),
                TelegramPhoto photo => await telegramBotClient.SendPhotoAsync(chatId, photo,
                    previousMessageId, chatHistoryLevel),
                TelegramAudio audio => await telegramBotClient.SendAudioAsync(chatId, audio,
                    previousMessageId, chatHistoryLevel),
                TelegramVideo video => await telegramBotClient.SendVideoAsync(chatId, video,
                    previousMessageId, chatHistoryLevel),
                TelegramVoice voice => await telegramBotClient.SendVoiceAsync(chatId, voice,
                    previousMessageId, chatHistoryLevel),
                TelegramDocument document => await telegramBotClient.SendDocumentAsync(chatId, document,
                    previousMessageId, chatHistoryLevel),
                _ => throw new InvalidEnumArgumentException()
            };

            if (messageWriter is not null)
            {
                await messageWriter.WriteAsync(chatId, sentMessage.Text.EmptyIfNull(), MessageDirection.Out,
                    cancellationToken);
            }

            await chatAccessor.SetPreviousMessageIdAsync(chatId, sentMessage.MessageId, cancellationToken);

            context.Result = new OkResult();
        }
    }
}