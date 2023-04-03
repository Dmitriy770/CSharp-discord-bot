namespace DiscordBot.Dal.Entities;

public sealed record VoiceUserEntity(
    ulong Id,
    string? Name,
    byte? Limit
);