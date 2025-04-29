using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMvc.Controllers;

namespace TelegramMvc.Examples.PageNavigationBot.Areas.Bot.Controllers;

[Area("Bot")]
public class StartController : TelegramController
{
    public IActionResult Index()
    {
        return TelegramMessage().AddKeyboard(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Submit a ticket", "/Bot/Tickets/Title") },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Test Forward", "/Forward"),
                InlineKeyboardButton.WithCallbackData("Attachment Replier", "/Attachments"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Privacy & Policies", "/Bot/Privacy"),
                InlineKeyboardButton.WithCallbackData("Contact Us", "/Bot/Contact")
            },
        });
    }
}