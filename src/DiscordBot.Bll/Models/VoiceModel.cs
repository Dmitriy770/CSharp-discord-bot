namespace DiscordBot.Bll.Models;

public record VoiceModel(
    ulong Id,
    IEnumerable<ulong> UserIDs
);
