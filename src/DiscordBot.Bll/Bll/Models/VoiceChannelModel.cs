namespace DiscordBot.Bll.Bll.Models;

public record VoiceChannelModel(
    ulong GuildId,
    ulong? Id,
    ulong[] UsersIds
);