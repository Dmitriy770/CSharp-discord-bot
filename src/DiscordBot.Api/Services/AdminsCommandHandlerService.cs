using Discord;
using Discord.WebSocket;
using DiscordBot.Bll.Bll.Commands;
using MediatR;

namespace DiscordBot.Api.Services;

public class AdminsCommandHandlerService
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceScopeFactory _serviceScope;
    private readonly CancellationToken _cancellationToken;

    public AdminsCommandHandlerService(
        DiscordSocketClient client, IServiceScopeFactory serviceScope)
    {
        _client = client;
        _serviceScope = serviceScope;
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
        _client.SlashCommandExecuted += AdminsCommandHandlerAsync;
        await Task.CompletedTask;
    }

    private async Task AdminsCommandHandlerAsync(SocketSlashCommand command)
    {
        if (command is { CommandName: "voice-admin", User: IGuildUser guildUser })
        {
            var guild = guildUser.Guild;
            var subcomand = command.Data.Options.First();
            var response = "";
            switch (subcomand.Name)
            {
                case "create":
                    var category = await guild.CreateCategoryAsync("VOICE CHANNELS");
                    var createVoiceChannel = await guild.CreateVoiceChannelAsync("create voice channel",
                        s => { s.CategoryId = category.Id; });
                    await Mediator.Send(new SetCreateVoiceChannelCommand(guild.Id, createVoiceChannel.Id));
                    break;
                case "set" when subcomand.Options.First().Name == "create-voice-channel":
                    var channel = subcomand.Options.First().Options.First().Value as IChannel;
                    if (channel is IVoiceChannel voiceChannel)
                    {
                        await Mediator.Send(new SetCreateVoiceChannelCommand(guild.Id, voiceChannel.Id),
                            _cancellationToken);
                        response = $"New channel to create channels: {voiceChannel.Mention}";
                    }
                    else
                    {
                        response = "This channel is not a voice channel";
                    }

                    break;
            }

            await command.RespondAsync(response, ephemeral: true);
        }
    }
}