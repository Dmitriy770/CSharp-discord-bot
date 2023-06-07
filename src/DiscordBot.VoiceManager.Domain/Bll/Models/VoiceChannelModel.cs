namespace DiscordBot.VoiceManager.Domain.Bll.Models;

public record VoiceChannelModel(
    ulong GuildId,
    ulong? Id,
    ulong[] UsersIds
);