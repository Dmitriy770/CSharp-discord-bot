using Discord;
using Discord.WebSocket;
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
        Console.WriteLine(_configuration.GetValue<int>("Aboba"));
        Console.WriteLine(_configuration["Token"]);

        
        var config = new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Verbose,
            MessageCacheSize = 1000
        };
        
        services.AddSingleton(config);
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<StartupService>();
        
        return services;
    }
}