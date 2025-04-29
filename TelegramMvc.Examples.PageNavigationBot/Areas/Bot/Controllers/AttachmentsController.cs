using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMvc.Controllers;
using TelegramMvc.Models.Views;

namespace TelegramMvc.Examples.PageNavigationBot.Areas.Bot.Controllers;

[Area("Bot")]
public class AttachmentsController : TelegramController
{
    public IActionResult Index()
    {
        return GetTelegramView()
            .AddKeyboard(InlineKeyboardButton.WithCallbackData("Home", "/bot/start"));
    }

    private TelegramView GetTelegramView()
    {
        var message = Update.Message;
        if (message is null)
            return TelegramMessage();
        if (message.Photo != null)
            return TelegramPhoto(InputFile.FromFileId(message.Photo.Last().FileId));
        if (message.Audio != null)
            return TelegramAudio(InputFile.FromFileId(message.Audio.FileId));
        if (message.Voice != null)
            return TelegramVoice(InputFile.FromFileId(message.Voice.FileId));
        if (message.Video != null)
            return TelegramVideo(InputFile.FromFileId(message.Video.FileId));
        if (message.Document != null)
            return TelegramDocument(InputFile.FromFileId(message.Document.FileId));
        return TelegramMessage("UnsupportedAttachment");
    }
}