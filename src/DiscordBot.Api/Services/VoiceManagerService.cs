using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Api.Options;
using DiscordBot.Domain.Bll.Models;
using DiscordBot.Domain.Bll.Services.Interfaces;
using DiscordBot.Domain.Entities;
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
            //TODO сделать команду доступной только на сервере
            var guildCommand = new SlashCommandBuilder()
                .WithName("voice")
                .WithDescription("Commands for managing the voice channel")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("claim")
                    .WithDescription("Command to capture a voice channel"))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Setting the parameters of the voice channel")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("limit")
                        .WithDescription("Sets the voice channel limit")
                        .AddOption("limit", ApplicationCommandOptionType.Integer, "Maximum number of users",
                            isRequired: true, minValue: 1, maxValue: 99))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Sets the voice channel name")
                        .AddOption("name", ApplicationCommandOptionType.String, "Voice channel name",
                            isRequired: true, minLength: 1, maxLength: 20)))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("reset")
                    .WithDescription("Resets voice channel parameters")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("limit")
                        .WithDescription("Resets voice channel limit"))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Resets voice channel name")));

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
                    await command.RespondAsync(subCommand.Name);
                    break;
                case "set":
                    var setCommand = subCommand.Options.First();
                    switch (setCommand.Name)
                    {
                        case "limit":
                            await command.RespondAsync(setCommand.Name);
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
                            await command.RespondAsync(resetCommand.Name);
                            break;
                        case "name":
                            await ResetVoiceNameHandler(command);
                            break;
                    }

                    break;
            }
        }
    }

    private async Task SetVoiceNameHandler(SocketSlashCommand command)
    {
        var name = command.Data.Options.First().Options.First().Options.First().Value as string;

        try
        {
            var guildUser = command.User as SocketGuildUser;
            var user = new UserEntity(
                Id: command.User.Id,
                Name: guildUser?.DisplayName ?? command.User.Username
            );
            var voices = _voiceService.SetVoiceName(user, name);

            UpdateUserVoices(voices);

            await command.RespondAsync($"New voice channel name: {name}");
        }
        catch (ArgumentException e)
        {
            await command.RespondAsync(e.Message);
        }
    }

    private async Task ResetVoiceNameHandler(SocketSlashCommand command)
    {
        var guildUser = command.User as SocketGuildUser;
        var user = new UserEntity(
            Id: command.User.Id,
            //TODO проверить нужен ли command.User.Username
            Name: guildUser?.DisplayName ?? command.User.Username
        );
        var voices = _voiceService.SetVoiceName(user, null);

        UpdateUserVoices(voices);

        await command.RespondAsync("Voice channel name reset");
    }

    private void UpdateUserVoices(UserVoiceModel voices)
    {
        foreach (var voiceId in voices.VoiceIDs)
        {
            var voiceChannel = _client.GetChannel(voiceId) as SocketVoiceChannel;
            voiceChannel?.ModifyAsync(x =>
            {
                x.Name = voices.Name;
                x.UserLimit = voices.Limit;
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
            var voiceParams = _voiceService.GetVoiceParams(guildUser.Id);

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