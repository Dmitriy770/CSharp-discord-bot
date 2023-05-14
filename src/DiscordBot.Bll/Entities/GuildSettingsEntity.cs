namespace DiscordBot.Bll.Entities;

public record GuildSettingsEntity(
    ulong Id,
    ulong? CreateVoiceChannelId
);