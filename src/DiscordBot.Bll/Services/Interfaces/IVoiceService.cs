using DiscordBot.Bll.Models;

namespace DiscordBot.Bll.Services.Interfaces;

public interface IVoiceService
{
    public UpdateVoicesModel ClaimVoice(UserModel user, ulong? voiceId, IEnumerable<ulong> userIDs);
    public UpdateVoicesModel SetVoiceLimit(UserModel user, int? limit);

    public UpdateVoicesModel SetVoiceName(UserModel user, string? name);

    public VoiceModel GetVoiceParams(UserModel user);

    public void SetUserVoice(ulong userId, ulong voiceId);

    public void RemoveUserVoice(ulong voiceId);
}