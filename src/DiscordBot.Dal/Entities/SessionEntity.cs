namespace DiscordBot.Dal.Entities;

public sealed record SessionEntity(
    ulong Id,
    ulong? VoiceId
);