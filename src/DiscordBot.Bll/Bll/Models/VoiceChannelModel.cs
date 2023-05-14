namespace DiscordBot.Bll.Bll.Models;

public record VoiceChannelModel(
    ulong GuildId,
    ulong Id,
    IEnumerable<ulong> UsersIds,
    ulong OwnerId
);