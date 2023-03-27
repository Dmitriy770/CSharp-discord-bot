namespace DiscordBot.Domain.Entities;

public sealed record VoiceUserEntity(
    ulong Id,
    string? Name,
    int? Limit
);