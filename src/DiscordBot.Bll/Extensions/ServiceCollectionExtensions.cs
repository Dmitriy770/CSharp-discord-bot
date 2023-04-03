using DiscordBot.Bll.Services;
using DiscordBot.Bll.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceService, VoiceService>();
        return services;
    }
}
