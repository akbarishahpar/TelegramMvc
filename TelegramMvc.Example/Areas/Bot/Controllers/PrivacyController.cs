﻿using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMvc.Controllers;

namespace TelegramMvc.Example.Areas.Bot.Controllers;

[Area("Bot")]
public class PrivacyController : TelegramController
{
    public IActionResult Index()
    {
        return TelegramMessage().AddKeyboard(
            InlineKeyboardButton.WithCallbackData("Home", "/start")
        );
    }
}