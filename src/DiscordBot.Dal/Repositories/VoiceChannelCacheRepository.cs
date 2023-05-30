using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;
using DiscordBot.Dal.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DiscordBot.Dal.Repositories;

public class VoiceChannelCacheRepository : BaseCacheRepository, IVoiceChannelCacheRepository
{
    public VoiceChannelCacheRepository(IOptions<CacheOptions> cacheOptions) : base(cacheOptions.Value)
    {
    }

    public async Task Delete(ulong guildId, ulong voiceChannelId)
    {
        var connection = await GetAndOpenConnection();
        await connection.HashDeleteAsync($"guild:{guildId}", voiceChannelId);
    }

    public async Task<VoiceChannelCacheEntity> Get(ulong guildId, ulong voiceChannelId)
    {
        var connection = await GetAndOpenConnection();
        var userId = await connection.HashGetAsync($"guild:{guildId}", voiceChannelId);
        return new VoiceChannelCacheEntity(
            guildId,
            voiceChannelId,
            userId.IsNull ? null : (ulong)userId
        );
    }

    public async Task Add(VoiceChannelCacheEntity voiceChannelCache)
    {
        var hash = new HashEntry[]
        {
            new(voiceChannelCache.Id, voiceChannelCache.OwnerId)
        };

        var connection = await GetAndOpenConnection();
        await connection.HashSetAsync($"guild:{voiceChannelCache.GuildId}", hash);
    }
}