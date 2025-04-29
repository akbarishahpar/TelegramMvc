using System.ComponentModel;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramMvc.Extensions;

public static class UpdateExtensions
{
    public static string? GetTextMessage(this Update update) => update.Type switch
    {
        UpdateType.Message => !string.IsNullOrEmpty(update.Message!.Text)
            ? update.Message!.Text
            : update.Message!.Caption,
        UpdateType.CallbackQuery => update.CallbackQuery!.Data,
        UpdateType.EditedMessage => !string.IsNullOrEmpty(update.EditedMessage!.Text)
            ? update.EditedMessage!.Text
            : update.EditedMessage!.Caption,
        _ => throw new InvalidEnumArgumentException()
    };

    public static Chat GetChat(this Update update) => update.Type switch
    {
        UpdateType.Message => update.Message!.Chat,
        UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat,
        UpdateType.EditedMessage => update.EditedMessage!.Chat,
        _ => throw new InvalidEnumArgumentException()
    };

    public static long GetChatId(this Update update) => update.GetChat().Id;

    public static bool IsTypeSupported(this Update update) => new[]
    {
        UpdateType.Message,
        UpdateType.CallbackQuery,
    }.Contains(update.Type);
}