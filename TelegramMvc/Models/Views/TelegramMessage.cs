namespace TelegramMvc.Models.Views;

public class TelegramMessage : TelegramResult<string>
{
    public TelegramMessage(string content) : base(content)
    {
    }
}