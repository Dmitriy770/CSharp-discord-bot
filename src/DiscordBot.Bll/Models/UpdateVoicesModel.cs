namespace DiscordBot.Bll.Models;

public record UpdateVoicesModel(
    VoiceModel Params,
    IEnumerable<ulong> VoiceIDs
);