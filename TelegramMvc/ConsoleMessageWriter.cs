using TelegramMvc.Contracts;
using TelegramMvc.Enums;

namespace TelegramMvc;

public class ConsoleMessageWriter : IMessageWriter
{
    public Task WriteAsync(long chatId, string message, MessageDirection direction, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{DateTime.Now}] [{direction}] {message}");
        return Task.CompletedTask;
    }
}