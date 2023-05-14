using DiscordBot.Bll.Bll.Services;
using DiscordBot.Bll.Bll.Services.Interfaces;
using DiscordBot.Bll.Interfaces;
using DiscordBot.Dal.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceChannelService, VoiceChannelService>();
        services.AddSingleton<ISettingsRepository, SettingRepository>();
        
        return services;
    }
}