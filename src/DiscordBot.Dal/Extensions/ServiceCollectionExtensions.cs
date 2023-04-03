using DiscordBot.Dal.Repositories;
using DiscordBot.Dal.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IVoiceUsersRepository, VoiceUsersRepository>();
        services.AddSingleton<ISessionRepository, SessionRepository>();
        
        return services;
    }
}