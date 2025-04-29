using System.Net;
using Telegram.Bot;
using TelegramMvc.Models.Settings;

namespace TelegramMvc.Factories;

public static class TelegramBotClientFactory
{
    public static TelegramBotClient Create(BotSettings botSettings)
    {
        var httpClient = CreateHttpClient(botSettings);
        return new TelegramBotClient(botSettings.Token, httpClient);
    }

    public static HttpClient CreateHttpClient(BotSettings botSettings)
    {
        var httpClient = new HttpClient();

        if (botSettings.ProxySettings == null) return httpClient;

        var proxy = new WebProxy(botSettings.ProxySettings.Server, botSettings.ProxySettings.Port)
        {
            Credentials = !string.IsNullOrEmpty(botSettings.ProxySettings.UserName) &&
                          !string.IsNullOrEmpty(botSettings.ProxySettings.Password)
                ? new NetworkCredential(botSettings.ProxySettings.UserName, botSettings.ProxySettings.Password)
                : null
        };

        return new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = true });
    }
}