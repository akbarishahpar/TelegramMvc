using Telegram.Bot.Types;

namespace TelegramMvc.Contracts
{
    public interface IChatAccessor
    {
        //Path related methods
        public Task<string> GetPathAsync(long chatId, CancellationToken cancellationToken);
        public Task<string> SetPathAsync(long chatId, string path, CancellationToken cancellationToken);
        
        //Previous message related methods
        public Task ForgetPreviousMessageIdAsync(long chatId, CancellationToken cancellationToken);
        public Task<int?> GetPreviousMessageIdAsync(long chatId, CancellationToken cancellationToken);
        public Task<int> SetPreviousMessageIdAsync(long chatId, int messageId, CancellationToken cancellationToken);
        
        //Profile related methods
        public Task SetProfileAsync(Chat chat, CancellationToken cancellationToken);
    }
}
