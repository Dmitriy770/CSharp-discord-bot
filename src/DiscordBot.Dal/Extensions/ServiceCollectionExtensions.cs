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
        services.AddSingleton<IVoiceChannelRepository, VoiceChannelRepository>();
        services.AddSingleton<ISettingsRepository, SettingRepository>();
        services.AddSingleton<IVoiceChannelSettingsRepository, VoiceChannelSettingsRepository>();
        
        return services;
    }

    public static IServiceCollection AddDalInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.Configure<DalOptions>(config.GetSection(nameof(DalOptions)));

        Postgres.MapCompositeTypes();
        
        Postgres.AddMigrations(services);

        return services;
    }
}