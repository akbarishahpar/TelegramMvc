using Telegram.Bot.Types;

namespace TelegramMvc.Models.Views;

public class TelegramVideo : TelegramResult<InputFile>
{
    public string? Caption { get; set; }

    public TelegramVideo(InputFile content) : base(content)
    {
    }

    public TelegramVideo(InputFile content, string caption) : base(content)
    {
        Caption = caption;
    }        
}