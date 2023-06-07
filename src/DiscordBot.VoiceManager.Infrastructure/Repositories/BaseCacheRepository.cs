using DiscordBot.VoiceManager.Infrastructure.Settings;
using StackExchange.Redis;

namespace DiscordBot.VoiceManager.Infrastructure.Repositories;

public abstract class BaseCacheRepository
{
    private readonly CacheOptions _cacheSettings;

    protected BaseCacheRepository(CacheOptions cacheSettings)
    {
        _cacheSettings = cacheSettings;
    }
    
    protected async Task<IDatabase> GetAndOpenConnection()
    {
        var options = new ConfigurationOptions()
        {
            EndPoints =
            {
                { _cacheSettings.Host, _cacheSettings.Port }
            },
            Password = _cacheSettings.Password
        };
        
        var redis = await ConnectionMultiplexer.ConnectAsync(options);
        return redis.GetDatabase();
    }
}