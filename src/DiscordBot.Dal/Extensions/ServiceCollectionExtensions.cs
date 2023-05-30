using DiscordBot.Bll.Interfaces;
using DiscordBot.Dal.Infrastructure;
using DiscordBot.Dal.Repositories;
using DiscordBot.Dal.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceChannelCacheRepository, VoiceChannelCacheRepository>();
        services.AddSingleton<IGuildSettingsRepository, GuildSettingRepository>();
        services.AddSingleton<IVoiceChannelSettingsRepository, VoiceChannelSettingsRepository>();
        
        return services;
    }

    public static IServiceCollection AddDalInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<DatabaseOptions>(config.GetSection(nameof(DatabaseOptions)));
        services.Configure<CacheOptions>(config.GetSection(nameof(CacheOptions)));

        Postgres.MapCompositeTypes();
        
        Postgres.AddMigrations(services);

        return services;
    }
}