using DiscordBot.Bll.Bll.Models;

namespace DiscordBot.Bll.Bll.Services.Interfaces;

public interface IVoiceChannelSettingsService
{
    public void Set(ulong guildId, ulong userId, VoiceChannelSettingsModel settings);

    public VoiceChannelSettingsModel Get(ulong guildId, ulong userId);
}