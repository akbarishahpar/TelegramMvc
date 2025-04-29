using TelegramMvc.Enums;

namespace TelegramMvc.Models.Settings;

public class BotSettings
{
    public string AreaName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string WebhookEndpoint { get; set; } = string.Empty;
    public RateLimitSettings? RateLimitSettings { get; set; }
    public ProxySettings? ProxySettings { get; set; }
    public ChatHistoryLevel ChatHistoryLevel { get; set; }
}