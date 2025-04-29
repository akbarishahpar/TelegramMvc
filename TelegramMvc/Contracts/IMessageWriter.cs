using Telegram.Bot.Types;
using TelegramMvc.Enums;

namespace TelegramMvc.Contracts;

public interface IMessageWriter
{
    Task WriteAsync(long chatId, string message, MessageDirection direction, CancellationToken cancellationToken);
}