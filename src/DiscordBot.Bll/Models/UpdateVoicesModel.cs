namespace DiscordBot.Bll.Models;

public record UpdateVoicesModel(
    VoiceParamsModel Params,
    IEnumerable<ulong> VoiceIDs
);
