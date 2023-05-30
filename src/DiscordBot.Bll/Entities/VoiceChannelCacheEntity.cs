namespace DiscordBot.Bll.Entities;

public record VoiceChannelCacheEntity(
    ulong GuildId,
    ulong Id,
    ulong? OwnerId
);