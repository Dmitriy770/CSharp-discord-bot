using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;

namespace DiscordBot.Dal.Repositories;

public class SettingRepository : ISettingsRepository
{
    public GuildSettingsEntity GetSettings(ulong guildId)
    {
        throw new NotImplementedException();
    }

    public void SetSettings(GuildSettingsEntity settings)
    {
        throw new NotImplementedException();
    }

    public VoiceChannelSettingsEntity GetVoiceChannelSettings(ulong guildId, ulong userId)
    {
        throw new NotImplementedException();
    }

    public void SetVoiceChannelSettings(VoiceChannelSettingsEntity settings)
    {
        throw new NotImplementedException();
    }
}