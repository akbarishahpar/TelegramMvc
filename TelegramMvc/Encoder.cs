using System.Collections.Concurrent;

namespace TelegramMvc;

public class Encoder
{
    private readonly ConcurrentDictionary<string, string> _storage = new();

    public string Push(string plain)
    {
        var code = $"encode:{Guid.NewGuid()}";
        _storage[code] = plain;
        return code;
    }

    public string Pop(string code)
    {
        if (!code.StartsWith("encode:")) 
            return code;

        if (!_storage.ContainsKey(code))
            return "/start";

        _storage.Remove(code, out var plain);
        
        return plain!;
    }
}