using Telegram.Bot.Types;

namespace TelegramMvc.Models.Views;

public class TelegramVoice : TelegramResult<InputFile>
{
    public string? Caption { get; set; }

    public TelegramVoice(InputFile content) : base(content)
    {
    }

    public TelegramVoice(InputFile content, string caption) : base(content)
    {
        Caption = caption;
    }        
}