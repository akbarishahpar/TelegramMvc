using System.Collections.Concurrent;

namespace TelegramMvc;

public class RateLimiter
{
    private readonly ConcurrentDictionary<long, DateTime> _sessions = new();

    public bool ShouldLimit(long chatId, double? rate)
        => rate != null &&
           _sessions.ContainsKey(chatId) &&
           (DateTime.Now - _sessions[chatId]).TotalSeconds < rate;

    public void UpdateLastAccessTime(long chatId)
    {
        _sessions[chatId] = DateTime.Now;

        //Expiring sessions
        foreach (var (id, lastAccessTime) in _sessions)
        {
            if (DateTime.Now - lastAccessTime > TimeSpan.FromMinutes(1))
                _sessions.Remove(id, out _);
        }
    }
}