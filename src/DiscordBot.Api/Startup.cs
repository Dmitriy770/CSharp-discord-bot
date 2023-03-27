using Discord;
using Discord.WebSocket;
using DiscordBot.Api.Options;
using DiscordBot.Api.Services;
using DiscordBot.Infrastructure.DependencyInjection;

namespace DiscordBot.Api;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        var config = new DiscordSocketConfig()
        {
            LogLevel = GetDiscordLogLevel(),
            MessageCacheSize = 1000
        };

        services
            .Configure<GuildOptions>(_configuration.GetSection("Guild"))
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<StartupService>()
            .AddSingleton<LogService>()
            .AddSingleton<VoiceManagerService>()
            .AddDomain()
            .AddInfrastructure();

        return services;
    }

    private LogSeverity GetDiscordLogLevel()
    {
        var logSeverity = LogSeverity.Error;
        if (Enum.TryParse(_configuration["Discord:LogLevel"], out LogSeverity logSeverityConfig))
        {
            logSeverity = logSeverityConfig;
        }

        return logSeverity;
    }
}