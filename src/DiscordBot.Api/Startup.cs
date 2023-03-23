using Discord;
using Discord.WebSocket;
using DiscordBot.Api.Options;
using DiscordBot.Api.Services;

namespace DiscordBot.Api;

public class Startup
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

        services.Configure<GuildOptions>(_configuration.GetSection("Guild"));
        services.AddSingleton(config);
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<StartupService>();
        services.AddSingleton<LogService>();
        services.AddSingleton<VoiceManagerService>();

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