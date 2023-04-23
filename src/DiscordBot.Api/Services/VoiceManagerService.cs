using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Api.Extensions;
using DiscordBot.Api.Options;
using DiscordBot.Bll.Exceptions;
using DiscordBot.Bll.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DiscordBot.Api.Services;

public sealed class VoiceManagerService
{
    private readonly DiscordSocketClient _client;
    private readonly IVoiceService _voiceService;
    private SocketGuild? _guild;
    private SocketVoiceChannel? _createVoiceChannel;

    public VoiceManagerService(DiscordSocketClient client, IOptionsMonitor<GuildOptions> options,
        IVoiceService voiceService)
    {
        _client = client;
        _voiceService = voiceService;
        _client.UserVoiceStateUpdated += CreateNewVoiceAsync;
        _client.UserVoiceStateUpdated += DeleteEmptyVoiceAsync;
        _client.SlashCommandExecuted += SlashCommandHandler;

        _client.Ready += async () =>
        {
            _guild = _client.GetGuild(options.CurrentValue.Id);
            _createVoiceChannel = _client.GetChannel(options.CurrentValue.CreateVoiceId) as SocketVoiceChannel;

            var guildCommand = new SlashCommandBuilder()
                .AddVoiceManagerCommands();

            try
            {
                await _client.Rest.CreateGuildCommand(guildCommand.Build(), _guild.Id);
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        };

        options.OnChange(x =>
        {
            _guild = _client.GetGuild(x.Id);
            _createVoiceChannel = _client.GetChannel(x.CreateVoiceId) as SocketVoiceChannel;
        });
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        if (command.CommandName == "voice")
        {
            var subCommand = command.Data.Options.First();
            switch (subCommand.Name)
            {
                case "claim":
                    await ClaimVoiceHandler(command);
                    break;
                case "set":
                    var setCommand = subCommand.Options.First();
                    switch (setCommand.Name)
                    {
                        case "limit":
                            await SetVoiceLimitHandler(command);
                            break;
                        case "name":
                            await SetVoiceNameHandler(command);
                            break;
                    }

                    break;
                case "reset":
                    var resetCommand = subCommand.Options.First();
                    switch (resetCommand.Name)
                    {
                        case "limit":
                            await ResetVoiceLimitHandler(command);
                            break;
                        case "name":
                            await ResetVoiceNameHandler(command);
                            break;
                    }

                    break;
            }
        }
    }

    private async Task ClaimVoiceHandler(SocketSlashCommand command)
    {
        if (command.User is SocketGuildUser guildUser)
        {
            try
            {
                var voiceChannel = guildUser.VoiceChannel;
                var userIDs = voiceChannel?.ConnectedUsers
                    .Select(x => x.Id).ToArray();
                
                _voiceService.ClaimVoice(
                    guildUser.Id,
                    voiceChannel?.Id,
                    userIDs);

                var properties = _voiceService.GetProperties(guildUser.Id);
                properties.Name = properties.Name.IsSpecified ? properties.Name : $"{guildUser.DisplayName}'s channel";
                voiceChannel?.UpdateAsync(properties);

                await command.RespondAsync("Voice claimed!", ephemeral: true);
            }
            catch (OwnerInVoiceException e)
            {
                await command.RespondAsync(e.Message, ephemeral: true);
            }
            catch (UserNotInVoiceException e)
            {
                await command.RespondAsync(e.Message, ephemeral: true);
            }
            catch (VoiceClaimedException e)
            {
                await command.RespondAsync(e.Message, ephemeral: true);
            }
        }
        else
        {
            await command.RespondAsync("You must be a server member", ephemeral: true);
        }
    }

    private async Task SetVoiceLimitHandler(SocketSlashCommand command)
    {
        if (command.User is SocketGuildUser guildUser)
        {
            var userLimit = Convert.ToByte(command.Data.Options.First().Options.First().Options.First().Value);
            try
            {
                var properties = new VoiceChannelProperties()
                {
                    UserLimit = userLimit
                };

                _voiceService.SetOrUpdateProperties(guildUser.Id, properties);

                var voiceId = _voiceService.GetUserVoice(guildUser.Id);

                if (voiceId is not null && _client.GetChannel(voiceId.Value) is SocketVoiceChannel channel)
                {
                    await channel.UpdateAsync(properties);
                }

                await command.RespondAsync($"New voice channel limit: {userLimit}", ephemeral: true);
            }
            catch (ArgumentOutOfRangeException e)
            {
                await command.RespondAsync(e.Message, ephemeral: true);
            }
        }
        else
        {
            await command.RespondAsync("You must be a server member", ephemeral: true);
        }
    }

    private async Task ResetVoiceLimitHandler(SocketSlashCommand command)
    {
        if (command.User is SocketGuildUser guildUser)
        {
            var properties = new VoiceChannelProperties()
            {
                UserLimit = null
            };
            _voiceService.SetOrUpdateProperties(guildUser.Id, properties);

            var voiceId = _voiceService.GetUserVoice(guildUser.Id);

            if (voiceId is not null && _client.GetChannel(voiceId.Value) is SocketVoiceChannel channel)
            {
                await channel.UpdateAsync(properties);
            }

            await command.RespondAsync("Voice channel limit reset", ephemeral: true);
        }
        else
        {
            await command.RespondAsync("You must be a server member", ephemeral: true);
        }
    }

    private async Task SetVoiceNameHandler(SocketSlashCommand command)
    {
        if (command.User is SocketGuildUser guildUser)
        {
            var name = command.Data.Options.First().Options.First().Options.First().Value as string;

            try
            {
                var properties = new VoiceChannelProperties()
                {
                    Name = name
                };

                _voiceService.SetOrUpdateProperties(guildUser.Id, properties);

                var voiceId = _voiceService.GetUserVoice(guildUser.Id);

                if (voiceId is not null && _client.GetChannel(voiceId.Value) is SocketVoiceChannel channel)
                {
                    await channel.UpdateAsync(properties);
                }

                await command.RespondAsync($"New voice channel name: {name}", ephemeral: true);
            }
            catch (ArgumentOutOfRangeException e)
            {
                await command.RespondAsync(e.Message, ephemeral: true);
            }
        }
        else
        {
            await command.RespondAsync("You must be a server member", ephemeral: true);
        }
    }

    private async Task ResetVoiceNameHandler(SocketSlashCommand command)
    {
        if (command.User is SocketGuildUser guildUser)
        {
            var properties = new VoiceChannelProperties
            {
                Name = null
            };
            _voiceService.SetOrUpdateProperties(guildUser.Id, properties);

            var voiceId = _voiceService.GetUserVoice(guildUser.Id);

            if (voiceId is not null && _client.GetChannel(voiceId.Value) is SocketVoiceChannel channel)
            {
                await channel.UpdateAsync(properties);
            }

            await command.RespondAsync("Voice channel name reset", ephemeral: true);
        }
        else
        {
            await command.RespondAsync("You must be a server member", ephemeral: true);
        }
    }

    private async Task CreateNewVoiceAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (_guild is not null && _createVoiceChannel is not null &&
            newVoiceState.VoiceChannel == _createVoiceChannel &&
            user is SocketGuildUser guildUser)
        {
            var properties = _voiceService.GetProperties(guildUser.Id);
            properties.CategoryId = _createVoiceChannel.CategoryId;
            properties.Name = properties.Name.IsSpecified ? properties.Name : $"{guildUser.DisplayName}'s channel";

            var newChannel = await _guild.CreateUserVoice(properties);

            _voiceService.SetUserVoice(user.Id, newChannel.Id);

            await guildUser.MoveAsync(newChannel.Id);
        }
    }

    private async Task DeleteEmptyVoiceAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (_createVoiceChannel is not null &&
            oldVoiceState.VoiceChannel?.CategoryId == _createVoiceChannel?.CategoryId &&
            oldVoiceState.VoiceChannel != _createVoiceChannel &&
            oldVoiceState.VoiceChannel?.ConnectedUsers.Count <= 0)
        {
            _voiceService.RemoveUserVoice(oldVoiceState.VoiceChannel.Id);

            await oldVoiceState.VoiceChannel.DeleteAsync();
        }
    }
}