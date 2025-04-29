namespace TelegramMvc.Models.Settings;

public class ProxySettings
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}