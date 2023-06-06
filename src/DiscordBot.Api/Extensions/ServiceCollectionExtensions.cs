using Discord.WebSocket;
using Discord;
using DiscordBot.Api.Options;
using DiscordBot.Api.Services;

namespace DiscordBot.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new DiscordSocketConfig()
        {
            LogLevel = GetDiscordLogLevel(configuration),
            MessageCacheSize = 1000
        };

        services
            .Configure<GuildOptions>(configuration.GetSection("Guild"))
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<StartupService>()
            .AddSingleton<LogService>()
            .AddSingleton<CommandHandlerService>()
            .AddSingleton<AdminsCommandHandlerService>()
            .AddSingleton<VoiceChannelService>()
            .AddSingleton<CreateVoiceChannelService>();

        return services;
    }
    
    private static LogSeverity GetDiscordLogLevel(IConfiguration configuration)
    {
        var logSeverity = LogSeverity.Error;
        if (Enum.TryParse(configuration["Discord:LogLevel"], out LogSeverity logSeverityConfig))
        {
            logSeverity = logSeverityConfig;
        }

        return logSeverity;
    }
}
