//TODO заменить модуль на сервис с командой, возвращающей информацию о боте
using Discord.Interactions;

namespace DiscordBot.Api.Modules;

public sealed class EchoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("echo", "return your message")]
    public async Task Echo(string input)
    {
        await RespondAsync(input, ephemeral: true);
    }
}