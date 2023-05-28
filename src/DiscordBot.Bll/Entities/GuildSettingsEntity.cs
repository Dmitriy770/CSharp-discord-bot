using System.Collections;

namespace DiscordBot.Bll.Entities;

public record GuildSettingsEntity
{
    public byte[] Id { get; init; } = Array.Empty<byte>();
    public byte[]? CreateVoiceChannelId { get; init; }
}