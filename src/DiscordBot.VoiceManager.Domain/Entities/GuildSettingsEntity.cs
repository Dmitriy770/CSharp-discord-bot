namespace DiscordBot.VoiceManager.Domain.Entities;

public record GuildSettingsEntity
{
    public byte[] Id { get; init; } = Array.Empty<byte>();
    public byte[]? CreateVoiceChannelId { get; init; }
}