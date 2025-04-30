using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMvc.Controllers;
using TelegramMvc.Extensions;

namespace TelegramMvc.Example.Areas.Bot.Controllers;

[Area("Bot")]
public class TicketsController : TelegramController
{
    private readonly Encoder _encoder;
    private readonly TicketService _ticketService;

    public TicketsController(Encoder encoder, TicketService ticketService)
    {
        _encoder = encoder;
        _ticketService = ticketService;
    }

    public IActionResult Title()
    {
        ViewBag.Title = (Update.Message?.Text).EmptyIfNull();

        if(string.IsNullOrEmpty(ViewBag.Title))
            return TelegramMessage("GetTitle");

        return TelegramMessage("ConfirmTitle")
            .AddKeyboard(InlineKeyboardButton.WithCallbackData("Next", _encoder.Push($"/tickets/body?title={ViewBag.Title}")));
    }

    public IActionResult Body(string title)
    {
        ViewBag.Title = title;
        ViewBag.Body = (Update.Message?.Text).EmptyIfNull();

        if (string.IsNullOrEmpty(ViewBag.Body))
            return TelegramMessage("GetBody").AddKeyboard(InlineKeyboardButton.WithCallbackData("Previous", _encoder.Push($"/tickets/title?title={title}")));

        return TelegramMessage("ConfirmBody")
            .AddKeyboard(InlineKeyboardButton.WithCallbackData("Previous", _encoder.Push($"/tickets/title?title={title}")))
            .AddKeyboard(InlineKeyboardButton.WithCallbackData("Submit", _encoder.Push($"/tickets/submit?title={title}&body={ViewBag.Body}")));
    }

    public IActionResult Submit(string title, string body)
    {
        ViewBag.TicketNumber = _ticketService.Add(title, body);
        return TelegramMessage()
            .AddKeyboard(InlineKeyboardButton.WithCallbackData("Home", "/start"));
    }
}