namespace DiscordBot.Domain.Entities;

public sealed record UserEntity(
    ulong Id,
    string Name);