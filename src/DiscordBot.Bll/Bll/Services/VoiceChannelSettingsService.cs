using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Bll.Services.Interfaces;

namespace DiscordBot.Bll.Bll.Services;

public class VoiceChannelSettingsService : IVoiceChannelSettingsService
{
    public void Set(ulong guildId, ulong userId, VoiceChannelSettingsModel settings)
    {
        throw new NotImplementedException();
    }

    public VoiceChannelSettingsModel Get(ulong guildId, ulong userId)
    {
        throw new NotImplementedException();
    }
}