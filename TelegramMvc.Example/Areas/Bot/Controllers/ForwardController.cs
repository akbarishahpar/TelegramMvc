using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramMvc.Controllers;
using TelegramMvc.Extensions;

namespace TelegramMvc.Example.Areas.Bot.Controllers;

[Area("Bot")]
public class ForwardController : TelegramController
{
    public async Task<IActionResult> Index()
    {
        var message = Update.Message?.Text;
        if (string.IsNullOrEmpty(message))
            return TelegramMessage();
        
        await TelegramBotClient.SendMessage(Update.GetChatId(), $"I got your message `<b>{message}</b>`! Now, forwarding...", ParseMode.Html);
        
        return TelegramRedirect("/start");
    }
}