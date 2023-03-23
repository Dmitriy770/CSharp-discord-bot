using Discord;
using Discord.WebSocket;
using DiscordBot.Api.Options;
using Microsoft.Extensions.Options;

namespace DiscordBot.Api.Services;

public sealed class VoiceManagerService
{
    private readonly DiscordSocketClient _client;
    private SocketGuild? _guild;
    private SocketVoiceChannel? _createVoiceChannel;

    public VoiceManagerService(DiscordSocketClient client, IOptionsMonitor<GuildOptions> options)
    {
        _client = client;
        _client.UserVoiceStateUpdated += CreateNewVoiceAsync;
        _client.UserVoiceStateUpdated += DeleteEmptyVoiceAsync;

        _client.Ready += () =>
        {
            _guild = _client.GetGuild(options.CurrentValue.Id);
            _createVoiceChannel = _client.GetChannel(options.CurrentValue.CreateVoiceId) as SocketVoiceChannel;
            return Task.CompletedTask;
        };

        options.OnChange(x =>
        {
            _guild = _client.GetGuild(x.Id);
            _createVoiceChannel = _client.GetChannel(x.CreateVoiceId) as SocketVoiceChannel;
        });
    }

    private async Task CreateNewVoiceAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (_guild is not null && _createVoiceChannel is not null &&
            newVoiceState.VoiceChannel == _createVoiceChannel &&
            user is SocketGuildUser guildUser)
        {
            var newVoiceChannel = await _guild.CreateVoiceChannelAsync(guildUser.DisplayName,
                x => { x.CategoryId = _createVoiceChannel.CategoryId; });

            await MoveUser(guildUser, newVoiceChannel.Id);
        }
    }

    private async Task DeleteEmptyVoiceAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if(_createVoiceChannel is not null &&
           oldVoiceState.VoiceChannel?.CategoryId == _createVoiceChannel?.CategoryId &&
           oldVoiceState.VoiceChannel != _createVoiceChannel &&
           oldVoiceState.VoiceChannel?.ConnectedUsers.Count <= 0)
        {
            await oldVoiceState.VoiceChannel.DeleteAsync();
        }
    }

    private static async Task MoveUser(IGuildUser user, ulong newChannelId)
    {
        await user.ModifyAsync(x => { x.ChannelId = newChannelId; });
    }
}