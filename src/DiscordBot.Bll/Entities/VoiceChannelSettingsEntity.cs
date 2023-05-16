namespace DiscordBot.Bll.Entities;

public record VoiceChannelSettingsEntity(
    ulong GuildId,
    ulong UserId,
    string? Name,
    int? UserLimit,
    int? Bitrate
);