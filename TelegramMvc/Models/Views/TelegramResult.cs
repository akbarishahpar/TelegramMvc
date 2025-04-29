using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramMvc.Models.Views;

public class TelegramResult
{
    public ReplyMarkup? ReplyMarkup { get; set; }
}

public class TelegramResult<T> : TelegramResult
{
    public T Content { get; }

    public TelegramResult(T content)
        => Content = content;
}