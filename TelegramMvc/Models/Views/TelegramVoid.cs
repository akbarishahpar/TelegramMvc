namespace TelegramMvc.Models.Views;

public class TelegramVoid : TelegramResult
{
    public bool TryDeleteHistory { get; }
    public TelegramVoid(bool tryDeleteHistory = true) => TryDeleteHistory = tryDeleteHistory;
}