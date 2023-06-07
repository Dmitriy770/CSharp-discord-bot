namespace DiscordBot.VoiceManager.Infrastructure.Settings;

public record DbOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Database { get; init; } = string.Empty;
    public bool Pooling { get; init; }
}