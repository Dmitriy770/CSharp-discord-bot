namespace DiscordBot.VoiceManager.Domain.Entities;

public record VoiceChannelCacheEntity(
    ulong GuildId,
    ulong Id,
    ulong? OwnerId
);