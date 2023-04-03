using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Api.Options;
using DiscordBot.Bll.Exceptions;
using DiscordBot.Bll.Models;
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
                .WithName("voice")
                .WithDescription("Commands for managing the voice channel")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("claim")
                    .WithDescription("Command to capture a voice channel")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Setting the parameters of the voice channel")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("limit")
                        .WithDescription("Sets the voice channel limit")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("limit", ApplicationCommandOptionType.Integer, "Maximum number of users",
                            isRequired: true, minValue: 1, maxValue: 99))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Sets the voice channel name")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("name", ApplicationCommandOptionType.String, "Voice channel name",
                            isRequired: true, minLength: 1, maxLength: 20)))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("reset")
                    .WithDescription("Resets voice channel parameters")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("limit")
                        .WithDescription("Resets voice channel limit")
                        .WithType(ApplicationCommandOptionType.SubCommand))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Resets voice channel name")
                        .WithType(ApplicationCommandOptionType.SubCommand)));
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
                var user = new UserModel(
                    Id: command.User.Id,
                    Name: guildUser.DisplayName
                );

                var voiceChannel = guildUser.VoiceChannel;
                var userIDs = voiceChannel?.ConnectedUsers.Select(x => x.Id).ToList() ?? new List<ulong>();
                var voiceModel = voiceChannel == null ? null : new VoiceModel(voiceChannel.Id, userIDs);

                var (voiceParams, voiceIDs) = _voiceService.ClaimVoice(user, voiceModel);
                UpdateUserVoices(voiceParams, voiceIDs);

                await command.RespondAsync($"Voice claimed", ephemeral: true);
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
            var limit = Convert.ToByte(command.Data.Options.First().Options.First().Options.First().Value);
            try
            {
                var user = new UserModel(
                    Id: command.User.Id,
                    Name: guildUser.DisplayName
                );

                var (voiceParams, voiceIDs) = _voiceService.SetVoiceLimit(user, limit);
                UpdateUserVoices(voiceParams, voiceIDs);

                await command.RespondAsync($"New voice channel limit: {limit}", ephemeral: true);
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
            var userModel = new UserModel(
                Id: command.User.Id,
                Name: guildUser.DisplayName
            );
            var (voiceParams, voiceIDs) = _voiceService.SetVoiceLimit(userModel, null);

            UpdateUserVoices(voiceParams, voiceIDs);

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
                var user = new UserModel(
                    Id: command.User.Id,
                    Name: guildUser.DisplayName
                );
                var (voiceParams, voiceIDs) = _voiceService.SetVoiceName(user, name);
                UpdateUserVoices(voiceParams, voiceIDs);

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
            var userModel = new UserModel(
                Id: command.User.Id,
                Name: guildUser.DisplayName
            );

            var (voiceParams, voiceIDs) = _voiceService.SetVoiceName(userModel, null);

            UpdateUserVoices(voiceParams, voiceIDs);

            await command.RespondAsync("Voice channel name reset", ephemeral: true);
        }
        else
        {
            await command.RespondAsync("You must be a server member", ephemeral: true);
        }
    }

    private void UpdateUserVoices(VoiceParamsModel voiceParams, IEnumerable<ulong> voiceIDs)
    {
        foreach (var voiceId in voiceIDs)
        {
            var voiceChannel = _client.GetChannel(voiceId) as SocketVoiceChannel;
            voiceChannel?.ModifyAsync(x =>
            {
                x.Name = voiceParams.Name;
                x.UserLimit = voiceParams.Limit;
            });
        }
    }

    private async Task CreateNewVoiceAsync(SocketUser user, SocketVoiceState oldVoiceState,
        SocketVoiceState newVoiceState)
    {
        if (_guild is not null && _createVoiceChannel is not null &&
            newVoiceState.VoiceChannel == _createVoiceChannel &&
            user is SocketGuildUser guildUser)
        {
            var userModel = new UserModel(
                Id: guildUser.Id,
                Name: guildUser.DisplayName
            );
            var voiceParams = _voiceService.GetVoiceParams(userModel);

            var newVoiceChannel = await _guild.CreateVoiceChannelAsync(voiceParams.Name,
                x =>
                {
                    x.CategoryId = _createVoiceChannel.CategoryId;
                    x.Name = voiceParams.Name;
                    x.UserLimit = voiceParams.Limit;
                });

            _voiceService.SetUserVoice(user.Id, newVoiceChannel.Id);

            await MoveUser(guildUser, newVoiceChannel.Id);
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

    private static async Task MoveUser(IGuildUser user, ulong newChannelId)
    {
        await user.ModifyAsync(x => { x.ChannelId = newChannelId; });
    }
}