namespace DiscordBot.Bll.Entities;

public record VoiceChannelSettingsEntity(
    ulong GuildId,
    ulong Id,
    string? Name,
    int? Limit,
    int? Bitrate
);