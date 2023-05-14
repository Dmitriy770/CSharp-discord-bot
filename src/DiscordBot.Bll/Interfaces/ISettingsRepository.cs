using DiscordBot.Bll.Entities;

namespace DiscordBot.Bll.Interfaces;

public interface ISettingsRepository
{
    public GuildSettingsEntity GetSettings(ulong guildId);

    public void SetSettings(GuildSettingsEntity settings);

    public VoiceChannelSettingsEntity GetVoiceChannelSettings(ulong guildId, ulong userId);

    public void SetVoiceChannelSettings(VoiceChannelSettingsEntity settings);
}