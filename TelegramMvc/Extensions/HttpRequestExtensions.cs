using Microsoft.AspNetCore.Http;
using Telegram.Bot.Types;

namespace TelegramMvc.Extensions;

public static class HttpRequestExtensions
{
    public static async Task<Update> ToTelegramUpdateAsync(this HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            httpRequest.EnableBuffering();
            var update =
                await httpRequest.Body.ReadJsonAsync<Update>((int)httpRequest.ContentLength!, cancellationToken);
            return update;
        }
        finally
        {
            httpRequest.Body.Position = 0;
        }
    }

    public static void SetPath(this HttpRequest request, string path)
    {
        var pathParts = path.Split('?');
        request.Path = PathString.FromUriComponent(pathParts[0]);
        if (pathParts.Length > 1)
            request.QueryString = QueryString.FromUriComponent("?" + pathParts[1]);
    }

    public static bool IsWebHookRequest(this HttpRequest request, string token) => request.Path == $"/bot/{token}";
}