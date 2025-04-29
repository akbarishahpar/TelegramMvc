using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramMvc.Enums;
using TelegramMvc.Models.Views;

namespace TelegramMvc.Extensions;

public static class TelegramBotClientExtensions
{
    public static async Task<Message> SendTextMessageAsync(this ITelegramBotClient telegramBotClient, long chatId,
        TelegramMessage message, int? lastSentMessageId = null,
        ChatHistoryLevel historyLevel = ChatHistoryLevel.Message)
    {
        var messageText = message.Content; // + SpecialNewLine;

        await telegramBotClient.TryDeleteHistoryAsync(chatId, lastSentMessageId, historyLevel);

        return await telegramBotClient.SendMessage(
            chatId,
            messageText,
            parseMode: ParseMode.Html,
            replyMarkup: message.ReplyMarkup
        );
    }

    public static async Task<Message> SendPhotoAsync(this ITelegramBotClient telegramBotClient, long chatId,
        TelegramPhoto photo, int? lastSentMessageId = null, ChatHistoryLevel historyLevel = ChatHistoryLevel.Message)
    {
        var messageText = photo.Caption;

        await telegramBotClient.TryDeleteHistoryAsync(chatId, lastSentMessageId, historyLevel);

        return await telegramBotClient.SendPhoto(
            chatId,
            photo.Content,
            caption: messageText,
            parseMode: ParseMode.Html,
            replyMarkup: photo.ReplyMarkup
        );
    }

    public static async Task<Message> SendAudioAsync(this ITelegramBotClient telegramBotClient, long chatId,
        TelegramAudio audio, int? lastSentMessageId = null, ChatHistoryLevel historyLevel = ChatHistoryLevel.Message)
    {
        var messageText = audio.Caption;

        await telegramBotClient.TryDeleteHistoryAsync(chatId, lastSentMessageId, historyLevel);

        return await telegramBotClient.SendPhoto(
            chatId,
            audio.Content,
            caption: messageText,
            parseMode: ParseMode.Html,
            replyMarkup: audio.ReplyMarkup
        );
    }

    public static async Task<Message> SendVideoAsync(this ITelegramBotClient telegramBotClient, long chatId,
        TelegramVideo video, int? lastSentMessageId = null, ChatHistoryLevel historyLevel = ChatHistoryLevel.Message)
    {
        var messageText = video.Caption;

        await telegramBotClient.TryDeleteHistoryAsync(chatId, lastSentMessageId, historyLevel);

        return await telegramBotClient.SendVideo(
            chatId,
            video.Content,
            caption: messageText,
            parseMode: ParseMode.Html,
            replyMarkup: video.ReplyMarkup
        );
    }

    public static async Task<Message> SendVoiceAsync(this ITelegramBotClient telegramBotClient, long chatId,
        TelegramVoice voice, int? lastSentMessageId = null, ChatHistoryLevel historyLevel = ChatHistoryLevel.Message)
    {
        var messageText = voice.Caption;

        await telegramBotClient.TryDeleteHistoryAsync(chatId, lastSentMessageId, historyLevel);

        return await telegramBotClient.SendVoice(
            chatId,
            voice.Content,
            caption: messageText,
            parseMode: ParseMode.Html,
            replyMarkup: voice.ReplyMarkup
        );
    }

    public static async Task<Message> SendDocumentAsync(this ITelegramBotClient telegramBotClient, long chatId,
        TelegramDocument document, int? lastSentMessageId = null, ChatHistoryLevel historyLevel = ChatHistoryLevel.Message)
    {
        var messageText = document.Caption;

        await telegramBotClient.TryDeleteHistoryAsync(chatId, lastSentMessageId, historyLevel);

        return await telegramBotClient.SendVoice(
            chatId,
            document.Content,
            caption: messageText,
            parseMode: ParseMode.Html,
            replyMarkup: document.ReplyMarkup
        );
    }

    public static async Task<bool> TryDeleteHistoryAsync(this ITelegramBotClient telegramBotClient, long chatId,
        int? messageId, ChatHistoryLevel historyLevel) => messageId is not null && historyLevel switch
    {
        ChatHistoryLevel.Message => await telegramBotClient.TryDeleteReplyMarkupAsync(chatId, messageId.Value),
        ChatHistoryLevel.None => await telegramBotClient.TryDeleteMessageAsync(chatId, messageId.Value),
        _ => false
    };

    private static async Task<bool> TryDeleteMessageAsync(this ITelegramBotClient telegramBotClient, long chatId,
        int messageId)
    {
        try
        {
            await telegramBotClient.DeleteMessage(chatId, messageId);
            return true;
        }
        catch (ApiRequestException)
        {
            return false;
        }
    }

    private static async Task<bool> TryDeleteReplyMarkupAsync(this ITelegramBotClient telegramBotClient, long chatId,
        int messageId)
    {
        try
        {
            await telegramBotClient.EditMessageReplyMarkup(
                chatId,
                messageId,
                replyMarkup: null
            );
            return true;
        }
        catch (ApiRequestException)
        {
            return false;
        }
    }
}