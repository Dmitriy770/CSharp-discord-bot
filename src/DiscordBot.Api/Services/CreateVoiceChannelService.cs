using Discord;
using Discord.WebSocket;
using DiscordBot.Api.Exceptions;
using DiscordBot.Bll.Bll.Queries;
using MediatR;

namespace DiscordBot.Api.Services;

public class CreateVoiceChannelService
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceScopeFactory _serviceScope;
    private readonly CancellationToken _token;

    public CreateVoiceChannelService(DiscordSocketClient client, IServiceScopeFactory serviceScope)
    {
        _client = client;
        _serviceScope = serviceScope;
        _token = new CancellationToken();
    }

    private IMediator Mediator
    {
        get
        {
            var scope = _serviceScope.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IMediator>();
        }
    }

    public async Task<IVoiceChannel> Get(ulong guildId)
    {
        var settings = await Mediator.Send(new GetGuildSettingsQuery(guildId), _token);

        if (settings.CreateVoiceChannelId is { } createVoiceChannelId)
        {
            var channel = await _client.GetChannelAsync(createVoiceChannelId);

            if (channel is IVoiceChannel voiceChannel)
            {
                return voiceChannel;
            }
        }

        throw new CreateVoiceChannelNotFoundException();
    }

    public async Task IsSet(ulong guildId)
    {
        var settings = await Mediator.Send(new GetGuildSettingsQuery(guildId), _token);

        if (settings.CreateVoiceChannelId is null)
        {
            throw new CreateVoiceChannelNotFoundException();
        }

        var channel = await _client.GetChannelAsync(settings.CreateVoiceChannelId.Value);
        if (channel is not IVoiceChannel)
        {
            throw new CreateVoiceChannelNotFoundException();
        }
    }
}