using Telegram.Bot.Types;

namespace TelegramMvc.Extensions;

public static class StringExtensions
{
    public static string EmptyIfNull(this string? value) => value ?? "";

    public static bool IsUrl(this string? str) =>
        str.EmptyIfNull().ToLower().StartsWith("/") || 
        str.EmptyIfNull().ToLower().StartsWith("encode:");
}