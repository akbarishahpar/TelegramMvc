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
            new[] { InlineKeyboardButton.WithCallbackData("Submit a ticket", "/tickets/title") },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Test Forward", "/forward"),
                InlineKeyboardButton.WithCallbackData("Attachment Replier", "/attachments"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Privacy & Policies", "/privacy"),
                InlineKeyboardButton.WithCallbackData("Contact Us", "/contact")
            },
        });
    }
}