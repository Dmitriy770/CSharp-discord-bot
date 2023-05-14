using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Bll.Services.Interfaces;

namespace DiscordBot.Bll.Bll.Services;

public class VoiceChannelService : IVoiceChannelService
{
    public void Add(ulong guildId, ulong voiceChannelOwnerId, ulong voiceChannelId)
    {
        throw new NotImplementedException();
    }

    public bool TryUpdate(VoiceChannelModel model, out VoiceChannelSettingsModel newSettings)
    {
        throw new NotImplementedException();
    }

    public void Delete(ulong guildId, ulong voiceChannelId)
    {
        throw new NotImplementedException();
    }

    public ulong Get(ulong guildId, ulong userId)
    {
        throw new NotImplementedException();
    }
}