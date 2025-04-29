namespace TelegramMvc.Models.Settings;

public class RateLimitSettings
{
    public string Message { get; set; } = string.Empty;
    public double DelayBetweenRequestsInSeconds { get; set; }
}