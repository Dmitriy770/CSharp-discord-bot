using DiscordBot.Bll.Models;

namespace DiscordBot.Bll.Services.Interfaces;

public interface IVoiceService
{
    public UpdateVoicesModel ClaimVoice(UserModel user, VoiceModel? voiceModel);
    public UpdateVoicesModel SetVoiceLimit(UserModel user, byte? limit);

    public UpdateVoicesModel SetVoiceName(UserModel user, string? name);

    public VoiceParamsModel GetVoiceParams(UserModel user);

    public void SetUserVoice(ulong userId, ulong voiceId);

    public void RemoveUserVoice(ulong voiceId);
}
