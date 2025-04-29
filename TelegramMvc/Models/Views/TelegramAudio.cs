using Telegram.Bot.Types;

namespace TelegramMvc.Models.Views;

public class TelegramAudio : TelegramResult<InputFile>
{
    public string? Caption { get; set; }

    public TelegramAudio(InputFile content) : base(content)
    {
    }

    public TelegramAudio(InputFile content, string caption) : base(content)
    {
        Caption = caption;
    }        
}