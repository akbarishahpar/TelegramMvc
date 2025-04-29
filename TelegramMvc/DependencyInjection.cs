using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramMvc.BackgroundServices;
using TelegramMvc.Factories;
using TelegramMvc.Middleware;
using TelegramMvc.Models.Settings;

namespace TelegramMvc;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureBotSettings(this IServiceCollection services,
        IConfiguration configuration, string configurationName)
    {
        var section = configuration.GetSection(configurationName);
        if (section is null)
            throw new Exception($"No section for BotSettings with name `{configurationName}` was found.");

        var botSettings = section.Get<BotSettings>();
        if (botSettings is null)
            throw new Exception($"Could not map provided section with name `{configurationName}` to BotSettings.");

        return services.AddSingleton(botSettings);
    }

    public static IServiceCollection SetWebhook(this IServiceCollection services)
    {
        return services.AddHostedService<WebhookBackgroundService>();
    }

    public static IServiceCollection AddRateLimiter(this IServiceCollection services)
        => services.AddSingleton<RateLimiter>();

    public static IServiceCollection AddEncoder(this IServiceCollection services)
        => services.AddSingleton<Encoder>();

    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
        => services.AddScoped<ITelegramBotClient>(sp =>
        {
            var botSettings = sp.GetRequiredService<BotSettings>();
            return TelegramBotClientFactory.Create(botSettings);
        });

    public static IServiceCollection AddTelegramMvc(this IServiceCollection services, IConfiguration configuration,
        string configurationName) => services
        .ConfigureBotSettings(configuration, configurationName)
        .SetWebhook()
        .AddRateLimiter()
        .AddEncoder()
        .AddTelegramBotClient();

    public static IServiceCollection AddTelegramMvc(this IServiceCollection services,
        Action<BotSettings> configureClient) => services
        .AddSingleton(_ =>
        {
            var botSettings = new BotSettings();
            configureClient(botSettings);
            return botSettings;
        })
        .SetWebhook()
        .AddRateLimiter()
        .AddEncoder()
        .AddTelegramBotClient();

    public static IApplicationBuilder UseTelegramMvc(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<TelegramMiddleware>();
}