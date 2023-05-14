namespace DiscordBot.Bll.Bll.Models;

public record GuildSettingsModel(
    ulong Id,
    ulong? CreateVoiceChannelId
);