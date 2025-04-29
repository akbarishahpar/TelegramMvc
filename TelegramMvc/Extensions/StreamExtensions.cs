using System.Text;
using System.Text.Json;

namespace TelegramMvc.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> ReadBytesAsync(this Stream stream, int length, CancellationToken cancellationToken)
    {
        var buffer = new byte[length];
        var readBytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        return buffer[..readBytes];
    }

    public static async Task<string> ReadStringAsync(this Stream stream, int length,
        CancellationToken cancellationToken)
        => Encoding.UTF8.GetString(await stream.ReadBytesAsync(length, cancellationToken));

    public static async Task<T> ReadJsonAsync<T>(this Stream stream, int length, CancellationToken cancellationToken)
    {
        var text = await stream.ReadStringAsync(length, cancellationToken);
        return JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Allow case-insensitive matching
            ReadCommentHandling = JsonCommentHandling.Skip, // If there are comments
            AllowTrailingCommas = true, // More forgiving
            WriteIndented = true // Pretty print if needed
        })!;
    }
}