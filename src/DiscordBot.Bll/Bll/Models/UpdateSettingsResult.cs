namespace DiscordBot.Bll.Bll.Models;

public record UpdateSettingsResult(
    ulong? UserChannelId,
    VoiceChannelSettingsModel VoiceChannelSettings
);