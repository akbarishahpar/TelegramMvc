using Telegram.Bot.Types;

namespace TelegramMvc.Models.Views;

public class TelegramRedirect : TelegramResult
{
    public string Path { get; }
    public TelegramRedirect(string path) => Path = path;
}