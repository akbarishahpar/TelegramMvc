using Telegram.Bot.Types;

namespace TelegramMvc.Models.Views;

public class TelegramDocument : TelegramResult<InputFile>
{
    public string? Caption { get; set; }

    public TelegramDocument(InputFile content) : base(content)
    {
    }

    public TelegramDocument(InputFile content, string caption) : base(content)
    {
        Caption = caption;
    }        
}