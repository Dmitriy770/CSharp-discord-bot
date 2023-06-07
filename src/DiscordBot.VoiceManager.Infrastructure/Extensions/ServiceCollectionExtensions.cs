using DiscordBot.VoiceManager.Domain.Interfaces;
using DiscordBot.VoiceManager.Infrastructure.Infrastructure;
using DiscordBot.VoiceManager.Infrastructure.Repositories;
using DiscordBot.VoiceManager.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.VoiceManager.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(this IServiceCollection services)
    {
        services.AddTransient<IVoiceChannelCacheRepository, VoiceChannelCacheRepository>();
        services.AddTransient<IGuildSettingsRepository, GuildSettingRepository>();
        services.AddTransient<IVoiceChannelSettingsRepository, VoiceChannelSettingsRepository>();
        
        return services;
    }

    public static IServiceCollection AddDalInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<DbOptions>(config.GetSection(nameof(DbOptions)));
        services.Configure<CacheOptions>(config.GetSection(nameof(CacheOptions)));

        Postgres.MapCompositeTypes();
        
        Postgres.AddMigrations(services);

        return services;
    }
}