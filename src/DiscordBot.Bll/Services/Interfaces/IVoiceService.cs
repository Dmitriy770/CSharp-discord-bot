using Discord;

namespace DiscordBot.Bll.Services.Interfaces;

public interface IVoiceService
{
    public void ClaimVoice(ulong userId, ulong? voiceId, IEnumerable<ulong> membersIds);

    public void SetOrUpdateProperties(ulong userId, VoiceChannelProperties properties);

    public VoiceChannelProperties GetProperties(ulong userId);

    public void SetUserVoice(ulong userId, ulong voiceId);

    public void RemoveUserVoice(ulong voiceId);

    public ulong? GetUserVoice(ulong userId);
}
