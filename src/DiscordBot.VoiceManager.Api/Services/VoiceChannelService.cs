using Discord;
using Discord.WebSocket;
using DiscordBot.VoiceManager.Api.Exceptions;
using DiscordBot.VoiceManager.Api.Extensions;
using DiscordBot.VoiceManager.Domain.Bll.Commands;
using DiscordBot.VoiceManager.Domain.Bll.Queries;
using MediatR;

namespace DiscordBot.VoiceManager.Api.Services;

public class VoiceChannelService
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceScopeFactory _serviceScope;
    private readonly CreateVoiceChannelService _createVoiceChannel;
    private readonly CancellationToken _cancellationToken;

    public VoiceChannelService(DiscordSocketClient client, IServiceScopeFactory serviceScope,
        CreateVoiceChannelService createVoiceChannel)
    {
        _client = client;
        _serviceScope = serviceScope;
        _createVoiceChannel = createVoiceChannel;
        _cancellationToken = new CancellationToken();
    }

    private IMediator Mediator
    {
        get
        {
            var scope = _serviceScope.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IMediator>();
        }
    }

    public async Task RunAsync()
    {
        _client.UserVoiceStateUpdated += CreateVoiceChannelAsync;
        _client.UserVoiceStateUpdated += DeleteEmptyVoiceChannelAsync;
        _client.UserVoiceStateUpdated += DeleteVoiceChannelOwnerAsync;

        await Task.CompletedTask;
    }

    private async Task CreateVoiceChannelAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (user is IGuildUser guildUser)
        {
            try
            {
                var createVoiceChannel = await _createVoiceChannel.Get(guildUser.GuildId);
                if (newVoiceState.VoiceChannel == createVoiceChannel)
                {
                    var voiceSettings = await Mediator.Send(
                        new GetVoiceChannelQuery(guildUser.GuildId, guildUser.Id),
                        _cancellationToken);

                    var newChannel = await guildUser.Guild.CreateVoiceChannelAsync(
                        voiceSettings.Name ?? $"{guildUser.DisplayName}`s channel",
                        s =>
                        {
                            s.CategoryId = createVoiceChannel.CategoryId;
                            s.UserLimit = voiceSettings.UsersLimit ?? createVoiceChannel.UserLimit;
                            s.Bitrate = Math.Min(voiceSettings.Bitrate ?? createVoiceChannel.Bitrate,
                                guildUser.Guild.MaxBitrate);
                        }
                    );

                    await guildUser.MoveAsync(newChannel);
                    await Mediator.Send(
                        new SetVoiceChannelOwnerCommand(
                            guildUser.GuildId,
                            guildUser.Id,
                            newChannel.Id),
                        _cancellationToken);
                }
            }
            catch (CreateVoiceChannelNotFoundException)
            {
            }
        }
    }

    private async Task DeleteEmptyVoiceChannelAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (user is IGuildUser guildUser && oldVoiceState.VoiceChannel is not null)
        {
            try
            {
                var createVoiceChannel = await _createVoiceChannel.Get(guildUser.GuildId);

                if (oldVoiceState.VoiceChannel != createVoiceChannel &&
                    oldVoiceState.VoiceChannel.CategoryId == createVoiceChannel.CategoryId &&
                    oldVoiceState.VoiceChannel.ConnectedUsers.Count <= 0)
                {
                    await oldVoiceState.VoiceChannel.DeleteAsync();
                }
            }
            catch (CreateVoiceChannelNotFoundException)
            {
            }
        }
    }

    private async Task DeleteVoiceChannelOwnerAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (user is IGuildUser guildUser && oldVoiceState.VoiceChannel is not null)
        {
            try
            {
                var createVoiceChannel = await _createVoiceChannel.Get(guildUser.GuildId);

                if (oldVoiceState.VoiceChannel != createVoiceChannel &&
                    oldVoiceState.VoiceChannel.CategoryId == createVoiceChannel.CategoryId)
                {
                    await Mediator.Send(
                        new DeleteVoiceChannelOwnerCommand(
                            guildUser.GuildId,
                            guildUser.Id,
                            oldVoiceState.VoiceChannel.Id),
                        _cancellationToken);
                }
            }
            catch (CreateVoiceChannelNotFoundException)
            {
            }
        }
    }
}