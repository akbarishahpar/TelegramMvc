using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMvc.Exceptions;
using TelegramMvc.Extensions;
using TelegramMvc.Models.Settings;

namespace TelegramMvc.Models.Views;

public class TelegramView : ActionResult
{
    public ControllerContext ControllerContext { get; set; }
    public HttpContext HttpContext { get; set; }
    public BotSettings BotSettings { get; set; }
    public Update Update { get; set; }
    public List<TelegramResult> Results { get; set; }

    public TelegramView(ControllerContext controllerContext, HttpContext httpContext, BotSettings botSettings,
        Update update, List<TelegramResult> results)
    {
        ControllerContext = controllerContext;
        HttpContext = httpContext;
        BotSettings = botSettings;
        Update = update;
        Results = results;
    }

    public TelegramView AddKeyboard(params KeyboardButton[] keyboardButtons) => SetReplyMarkup(
        this,
        new ReplyKeyboardMarkup(keyboardButtons)
        {
            OneTimeKeyboard = false,
            ResizeKeyboard = true,
            Selective = true
        }
    );

    public TelegramView AddKeyboard(KeyboardButton[][] keyboardButtons) => SetReplyMarkup(
        this,
        new ReplyKeyboardMarkup(keyboardButtons)
        {
            OneTimeKeyboard = false,
            ResizeKeyboard = true,
            Selective = true
        }
    );

    public TelegramView AddKeyboard(params InlineKeyboardButton[] keyboardButtons) =>
        SetReplyMarkup(this, new InlineKeyboardMarkup(keyboardButtons));

    public TelegramView AddKeyboard(InlineKeyboardButton[][] keyboardButtons) =>
        SetReplyMarkup(this, new InlineKeyboardMarkup(keyboardButtons));

    public TelegramView AddMessage(ViewResult viewResult)
    {
        Results.Add(new TelegramMessage(viewResult.ToHtml(HttpContext, ControllerContext)));
        return this;
    }

    public TelegramView AddPhoto(string path)
    {
        Results.Add(new TelegramPhoto(new InputFileUrl(path)));
        return this;
    }

    #region private methods

    private static TelegramView SetReplyMarkup(TelegramView telegramView, ReplyMarkup replyMarkup)
    {
        var lastResult = telegramView.Results.LastOrDefault();
        if (lastResult == null)
            throw new TelegramResultNotFoundException();
        lastResult.ReplyMarkup = replyMarkup;
        return telegramView;
    }

    #endregion
}