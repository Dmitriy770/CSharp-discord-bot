namespace DiscordBot.VoiceManager.Domain.Bll.Models;

public record GuildSettingsModel(
    ulong Id,
    ulong? CreateVoiceChannelId
);