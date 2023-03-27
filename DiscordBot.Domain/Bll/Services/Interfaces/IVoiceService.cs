using DiscordBot.Domain.Bll.Models;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Domain.Bll.Services.Interfaces;

public interface IVoiceService
{
    IEnumerable<ulong> SetVoiceLimit(ulong userId, int? limit);

    UserVoiceModel SetVoiceName(UserEntity user, string? name);

    UserVoiceModel GetVoiceParams(ulong userId);

    public IEnumerable<ulong> GetUserVoiceChannels(ulong userId);

    public void SetUserVoice(ulong userId, ulong voiceId);

    public void RemoveUserVoice(ulong voiceId);
}