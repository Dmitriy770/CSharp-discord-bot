//TODO убрать модуль
using Discord.Interactions;
using DiscordBot.Api.Services;

namespace DiscordBot.Api.Modules;

[EnabledInDm(false)]
[Group("voice", "Commands for managing the voice channel")]
public sealed class VoiceManagerModule : InteractionModuleBase<SocketInteractionContext>
{

    [SlashCommand("claim", "Command to capture a voice channel")]
    public async Task ClaimVoice()
    {
        await RespondAsync("Claim", ephemeral: true);
    }

    [Group("set", "Setting the parameters of the voice channel")]
    public class SetVoiceCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly VoiceManagerService _voiceManagerService;

        public SetVoiceCommands(VoiceManagerService voiceManagerService)
        {
            _voiceManagerService = voiceManagerService;
        }
        
        [SlashCommand("limit", "Sets the voice channel limit")]
        public async Task SetVoiceLimit(
            [Summary("limit", "Maximum number of users"), MinValue(1), MaxValue(99)]
            int limit)
        {
            await RespondAsync($"New voice limit: {limit}", ephemeral: true);
        }
        
        [SlashCommand("name", "Sets the voice channel name")]
        public async Task SetVoiceName(
            [Summary("name", "Voice channel name"), MinLength(1), MaxValue(20)]
            string name)
        {
            // _voiceManagerService.SetVoiceName(Context.User.Id, name);

            await RespondAsync($"New voice name: {name}", ephemeral: true);
        }
    }
    
    [Group("reset", "Resets voice channel parameters")]
    public class ResetVoiceCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("limit", "Resets voice channel limit")]
        public async Task ResetVoiceLimit()
        {
            await RespondAsync("Reset voice limit");
        }

        [SlashCommand("name", "Resets voice channel name")]
        public async Task ResetVoiceName()
        {
            await RespondAsync("Reset voice name");
        }
    }
}