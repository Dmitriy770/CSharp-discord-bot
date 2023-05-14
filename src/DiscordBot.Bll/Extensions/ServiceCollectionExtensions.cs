using DiscordBot.Bll.Bll.Services;
using DiscordBot.Bll.Bll.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceChannelSettingsService, VoiceChannelSettingsService>();
        services.AddSingleton<IVoiceChannelService, VoiceChannelService>();
        services.AddSingleton<IGuildSettingsService, GuildSettingsService>();
        return services;
    }
}