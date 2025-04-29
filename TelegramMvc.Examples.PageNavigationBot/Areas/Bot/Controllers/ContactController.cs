using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMvc.Controllers;

namespace TelegramMvc.Examples.PageNavigationBot.Areas.Bot.Controllers;

[Area("Bot")]
public class ContactController : TelegramController
{
    public IActionResult Index()
    {
        return TelegramMessage().AddKeyboard(
            InlineKeyboardButton.WithCallbackData("Home", "/Bot/Start")
        );
    }
}