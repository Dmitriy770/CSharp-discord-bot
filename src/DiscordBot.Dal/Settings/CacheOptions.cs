namespace DiscordBot.Dal.Settings;

public record CacheOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string Password { get; init; } = string.Empty;
}