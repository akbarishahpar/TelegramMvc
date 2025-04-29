using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using System.Text.Json;
using TelegramMvc.Factories;
using TelegramMvc.Models.Settings;

namespace TelegramMvc.BackgroundServices
{
    public class WebhookBackgroundService : BackgroundService
    {
        private readonly BotSettings _botSettings;

        public WebhookBackgroundService(BotSettings botSettings)
        {
            _botSettings = botSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var httpClient = TelegramBotClientFactory.CreateHttpClient(_botSettings);
            var url = $"https://api.telegram.org/bot{_botSettings.Token}/setWebhook";
            var _ = await httpClient.PostAsJsonAsync(url, new
            {
                url = $"{_botSettings.WebhookEndpoint}/bot/{_botSettings.Token}"
            }, stoppingToken);
        }
    }
}