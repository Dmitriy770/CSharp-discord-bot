using DiscordBot.Domain.Bll.Services;
using DiscordBot.Domain.Bll.Services.Interfaces;
using DiscordBot.Domain.Interfaces;
using DiscordBot.Infrastructure.Dal.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceService, VoiceService>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceUsersRepository, VoiceUsersRepository>();
        services.AddSingleton<ISessionRepository, SessionRepository>();
        return services;
    }
}