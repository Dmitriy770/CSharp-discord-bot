using DiscordBot.Dal.Settings;
using StackExchange.Redis;

namespace DiscordBot.Dal.Repositories;

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