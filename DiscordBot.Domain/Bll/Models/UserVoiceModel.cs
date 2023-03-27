namespace DiscordBot.Domain.Bll.Models;

public record UserVoiceModel(
    IEnumerable<ulong> VoiceIDs,
    string? Name,
    int? Limit
);