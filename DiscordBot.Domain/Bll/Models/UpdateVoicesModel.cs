namespace DiscordBot.Domain.Bll.Models;

public record UpdateVoicesModel(
    VoiceModel Params,
    IEnumerable<ulong> VoiceIDs
);