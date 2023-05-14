using DiscordBot.Bll.Bll.Models;

namespace DiscordBot.Bll.Bll.Services.Interfaces;

public interface IVoiceChannelService
{
    public void Add(ulong guildId, ulong voiceChannelOwnerId, ulong voiceChannelId);

    public bool TryUpdate(VoiceChannelModel model, out VoiceChannelSettingsModel newSettings);

    public void Delete(ulong guildId, ulong voiceChannelId);

    public ulong Get(ulong guildId, ulong userId);
}