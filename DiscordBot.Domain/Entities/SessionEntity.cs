namespace DiscordBot.Domain.Entities;

public sealed record SessionEntity(
    ulong Id,
    ulong? VoiceId
);