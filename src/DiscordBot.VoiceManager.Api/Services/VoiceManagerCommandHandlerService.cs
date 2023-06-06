using Discord;
using Discord.WebSocket;
using DiscordBot.VoiceManager.Api.Exceptions;
using DiscordBot.VoiceManager.Domain.Bll.Commands;
using DiscordBot.VoiceManager.Domain.Bll.Exceptions;
using DiscordBot.VoiceManager.Domain.Bll.Models;
using DiscordBot.VoiceManager.Domain.Bll.Queries;
using MediatR;

namespace DiscordBot.VoiceManager.Api.Services;

public class VoiceManagerCommandHandlerService
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceScopeFactory _serviceScope;
    private readonly CreateVoiceChannelService _voiceChannel;
    private readonly CancellationToken _cancellationToken;

    public VoiceManagerCommandHandlerService(
        DiscordSocketClient client, IServiceScopeFactory serviceScope, CreateVoiceChannelService voiceChannel)
    {
        _client = client;
        _serviceScope = serviceScope;
        _voiceChannel = voiceChannel;
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
        _client.SlashCommandExecuted += CommandHandlerAsync;
        await Task.CompletedTask;
    }

    private async Task CommandHandlerAsync(SocketSlashCommand command)
    {
        if (command is { CommandName: "voice", User: IGuildUser guildUser })
        {
            try
            {
                await _voiceChannel.IsSet(guildUser.GuildId);

                var subCommand = command.Data.Options.First();
                switch (subCommand.Name)
                {
                    case "info":
                        await InfoCommandHandlerAsync(command);
                        break;
                    case "claim":
                        await ClaimCommandHandlerAsync(command);
                        break;
                    case "set":
                        await SetCommandHandlerAsync(command);
                        break;
                    case "reset":
                        await ResetCommandHandlerAsync(command);
                        break;
                }
            }
            catch (CreateVoiceChannelNotFoundException)
            {
                await command.RespondAsync("You need to set up a channel to create other channels", ephemeral: true);
            }
        }
    }

    private async Task ClaimCommandHandlerAsync(SocketSlashCommand command)
    {
        try
        {
            var guildUser = (SocketGuildUser)command.User;

            var voiceChannel = guildUser.VoiceChannel;
            var userIDs = voiceChannel?.ConnectedUsers.Select(x => x.Id).ToArray();

            var voiceChannelModel = new VoiceChannelModel(
                guildUser.Guild.Id,
                voiceChannel?.Id,
                userIDs ?? Array.Empty<ulong>()
            );
            var settings = await Mediator.Send(
                new ClaimVoiceChannelCommand(guildUser.Guild.Id, guildUser.Id, voiceChannelModel),
                _cancellationToken);

            await UpdateUserChannelAsync(voiceChannelModel.Id, settings, guildUser);

            await command.RespondAsync("Voice claimed!", ephemeral: true);
        }
        catch (OwnerInVoiceException)
        {
            await command.RespondAsync("Owner in voice", ephemeral: true);
        }
        catch (UserNotInVoiceException)
        {
            await command.RespondAsync("You are not in the voice channel", ephemeral: true);
        }
        catch (VoiceClaimedException)
        {
            await command.RespondAsync("This is your voice channel", ephemeral: true);
        }
    }

    private async Task InfoCommandHandlerAsync(SocketSlashCommand command)
    {
        var guildUser = (IGuildUser)command.User;
        var settings = await Mediator.Send(new GetVoiceChannelQuery(guildUser.GuildId, guildUser.Id),
            _cancellationToken);

        var createVoiceChannel = await _voiceChannel.Get(guildUser.GuildId);

        var name = settings.Name ?? $"{guildUser.DisplayName}`s channel";
        var usersLimit = (settings.UsersLimit ?? createVoiceChannel.UserLimit) is null
            ? "unlimited"
            : $"{settings.UsersLimit ?? createVoiceChannel.UserLimit}";
        var bitrate =
            $"{Math.Min(settings.Bitrate ?? createVoiceChannel.Bitrate, guildUser.Guild.MaxBitrate) / 1000} kbps";

        var embed = new EmbedBuilder()
            .WithTitle($"{guildUser.DisplayName}`s channel")
            .WithColor(Color.Blue)
            .AddField("Name:", name)
            .AddField("Users limit:", usersLimit)
            .AddField("Bitrate:", bitrate);

        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
    }

    private async Task SetCommandHandlerAsync(SocketSlashCommand command)
    {
        var setCommand = command.Data.Options.First().Options.First();
        var guildUser = (IGuildUser)command.User;
        ulong? channelId;
        VoiceChannelSettingsModel settings;
        switch (setCommand.Name)
        {
            case "limit":
                var limit = Convert.ToInt32(setCommand.Options.First().Value);
                (channelId, settings) = await Mediator.Send(
                    new SetVoiceChannelUsersLimitCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id,
                        limit),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync($"New voice channel limit: {limit}", ephemeral: true);
                break;

            case "name":
                var name = setCommand.Options.First().Value as string;
                (channelId, settings) = await Mediator.Send(
                    new SetVoiceChannelNameCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id, name),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync($"New voice channel name: {name}", ephemeral: true);
                break;

            case "bitrate":
                var bitrate = Convert.ToInt32(setCommand.Options.First().Value);
                (channelId, settings) = await Mediator.Send(
                    new SetVoiceChannelBitrateCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id,
                        bitrate),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync($"New voice channel bitrate: {bitrate}", ephemeral: true);
                break;

            default:
                await command.RespondAsync($"Set command {setCommand.Name} not found", ephemeral: true);
                break;
        }
    }

    private async Task ResetCommandHandlerAsync(SocketSlashCommand command)
    {
        var resetCommand = command.Data.Options.First().Options.First();
        var guildUser = (IGuildUser)command.User;
        ulong? channelId;
        VoiceChannelSettingsModel settings;
        switch (resetCommand.Name)
        {
            case "limit":
                (channelId, settings) = await Mediator.Send(
                    new SetVoiceChannelUsersLimitCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id,
                        null),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync("Voice channel users limit reset", ephemeral: true);
                break;

            case "name":
                (channelId, settings) = await Mediator.Send(
                    new SetVoiceChannelNameCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id, null),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync("Voice channel name reset", ephemeral: true);
                break;

            case "bitrate":
                (channelId, settings) = await Mediator.Send(
                    new SetVoiceChannelBitrateCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id,
                        null),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync("Voice channel bitrate reset", ephemeral: true);
                break;

            case "all":
                (channelId, settings) = await Mediator.Send(
                    new ResetVoiceChannelCommand(guildUser.GuildId, guildUser.Id, guildUser.VoiceChannel?.Id),
                    _cancellationToken);
                await UpdateUserChannelAsync(channelId, settings, guildUser);
                await command.RespondAsync("Voice channel users limit reset", ephemeral: true);
                break;

            default:
                await command.RespondAsync($"Reset command {resetCommand.Name} not found", ephemeral: true);
                break;
        }
    }

    private async Task UpdateUserChannelAsync(ulong? voiceChannelId, VoiceChannelSettingsModel settings,
        IGuildUser user)
    {
        if (voiceChannelId is not null)
        {
            var channel = _client.GetChannel(voiceChannelId.Value);

            if (channel is IVoiceChannel voiceChannel)
            {
                var createVoiceChannel = await _voiceChannel.Get(user.GuildId);

                await voiceChannel.ModifyAsync(s =>
                {
                    s.Name = settings.Name ?? $"{user.DisplayName}`s channel";
                    s.UserLimit = settings.UsersLimit ?? createVoiceChannel.UserLimit;
                    s.Bitrate = Math.Min(settings.Bitrate ?? createVoiceChannel.Bitrate, user.Guild.MaxBitrate);
                });
            }
        }
    }
}