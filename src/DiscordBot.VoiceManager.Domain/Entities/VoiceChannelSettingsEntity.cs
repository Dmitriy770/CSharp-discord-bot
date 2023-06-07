namespace DiscordBot.VoiceManager.Domain.Entities;

public record VoiceChannelSettingsEntity
{
    public byte[] GuildId { get; init; } = Array.Empty<byte>();
    public byte[] UserId { get; init; } = Array.Empty<byte>();
    public string? Name { get; init; }
    public int? UsersLimit { get; init; }
    public int? Bitrate { get; init; }
};