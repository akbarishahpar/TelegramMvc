using System.Collections.Concurrent;
using Telegram.Bot.Types;
using TelegramMvc.Contracts;
using TelegramMvc.Extensions;
using TelegramMvc.Models.Entities;

namespace TelegramMvc;

public class InMemoryChatsRepository : IChatAccessor
{
    private static readonly ConcurrentDictionary<long, ChatBase> Chats = new();

    public Task<string> GetPathAsync(long chatId, CancellationToken cancellationToken)
    {
        var chatBase = GetOrCreateChat(chatId);
        return Task.FromResult(chatBase.Path);
    }

    public Task<string> SetPathAsync(long chatId, string path, CancellationToken cancellationToken)
    {
        var chatBase = GetOrCreateChat(chatId);
        chatBase.Path = path;
        return Task.FromResult(path);
    }

    public Task ForgetPreviousMessageIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var chatBase = GetOrCreateChat(chatId);
        chatBase.PreviousMessageId = null;
        return Task.CompletedTask;
    }

    public Task<int?> GetPreviousMessageIdAsync(long chatId, CancellationToken cancellationToken)
    {
        var chatBase = GetOrCreateChat(chatId);
        return Task.FromResult(chatBase.PreviousMessageId);
    }

    public Task<int> SetPreviousMessageIdAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        var chatBase = GetOrCreateChat(chatId);
        chatBase.PreviousMessageId = messageId;
        return Task.FromResult(messageId);
    }

    public Task SetProfileAsync(Chat chat, CancellationToken cancellationToken)
    {
        var chatBase = GetOrCreateChat(chat.Id);
        
        chatBase.FirstName = chat.FirstName.EmptyIfNull();
        chatBase.LastName = chat.LastName.EmptyIfNull();
        chatBase.UserName = chat.Username.EmptyIfNull();

        return Task.CompletedTask;
    }

    private static ChatBase GetOrCreateChat(long chatId)
    {
        if (!Chats.ContainsKey(chatId))
            Chats[chatId] = new ChatBase { Id = chatId };
        return Chats[chatId];
    }
}