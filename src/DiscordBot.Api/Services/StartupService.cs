using Discord;
using Discord.WebSocket;

namespace DiscordBot.Api.Services;

public sealed class StartupService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public StartupService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _configuration["Token"]);
        await _client.StartAsync();
    }
    
}