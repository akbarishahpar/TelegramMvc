namespace TelegramMvc.Models.Entities;

public class ChatBase
{
    public long Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public int? PreviousMessageId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}