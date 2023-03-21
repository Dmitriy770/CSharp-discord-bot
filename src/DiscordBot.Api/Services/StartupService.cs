using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Api.Services;

public class StartupService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _provider;

    public StartupService(DiscordSocketClient client, IConfiguration configuration, IServiceProvider provider)
    {
        _client = client;
        _configuration = configuration;
        _provider = provider;
    }

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _configuration["Token"]);
        await _client.StartAsync();
        
        _client.Ready += async () =>
        {
            var interactionService = new InteractionService(_client);

            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            await interactionService.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("GuildId"));

            _client.InteractionCreated += async interaction =>
            {
                var scope = _provider.CreateScope();
                var ctx = new SocketInteractionContext(_client, interaction);
                await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
            };
        };
    }
    
}