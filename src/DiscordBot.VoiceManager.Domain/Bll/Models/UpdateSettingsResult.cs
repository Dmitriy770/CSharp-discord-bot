namespace DiscordBot.VoiceManager.Domain.Bll.Models;

public record UpdateSettingsResult(
    ulong? UserChannelId,
    VoiceChannelSettingsModel VoiceChannelSettings
);