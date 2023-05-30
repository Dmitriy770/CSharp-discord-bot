using DiscordBot.Bll.Entities;

namespace DiscordBot.Bll.Interfaces;

public interface IVoiceChannelCacheRepository
{
    public Task Add(VoiceChannelCacheEntity voiceChannelCache);

    public Task Delete(ulong guildId, ulong voiceChannelId);
    
    public Task<VoiceChannelCacheEntity> Get(ulong guildId, ulong voiceChannelId);
}