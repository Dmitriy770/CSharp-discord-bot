using DiscordBot.Bll.Entities;

namespace DiscordBot.Bll.Interfaces;

public interface ISettingsRepository
{
    public GuildSettingsEntity GetSettings(ulong guildId);

    public void SetSettings(GuildSettingsEntity settings);

    public VoiceChannelSettingsEntity GetVoiceChannelParams(ulong guildId, ulong userId);

    public void SetVoiceChannelParams(VoiceChannelSettingsEntity settings);
}