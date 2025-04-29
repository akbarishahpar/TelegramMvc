using Telegram.Bot.Types;

namespace TelegramMvc.Models.Views;

public class TelegramPhoto : TelegramResult<InputFile>
{
    public string? Caption { get; set; }

    public TelegramPhoto(InputFile content) : base(content)
    {
    }

    public TelegramPhoto(InputFile content, string caption) : base(content)
    {
        Caption = caption;
    }        
}