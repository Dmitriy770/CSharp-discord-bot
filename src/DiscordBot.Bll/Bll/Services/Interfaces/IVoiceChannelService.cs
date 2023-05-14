using DiscordBot.Bll.Bll.Models;

namespace DiscordBot.Bll.Bll.Services.Interfaces;

public interface IVoiceChannelService
{
    public bool TryAdd(ulong guildId, ulong ownerId, ulong voiceChannelId);

    public VoiceChannelSettingsModel Update(VoiceChannelModel voiceChannel);

    public void Delete(ulong guildId, ulong voiceChannelId);

    public bool TryGet(ulong guildId, ulong userId, out ulong voiceChannelId);
}