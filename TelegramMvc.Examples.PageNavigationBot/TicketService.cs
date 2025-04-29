using TelegramMvc.Examples.PageNavigationBot.Models;

namespace TelegramMvc.Examples.PageNavigationBot;

public class TicketService
{
    public List<Ticket> Tickets { get; set; } = new();

    public int Add(string title, string body)
    {
        Tickets.Add(new Ticket { Title = title, Body = body });
        return Tickets.Count - 1;
    } 
}