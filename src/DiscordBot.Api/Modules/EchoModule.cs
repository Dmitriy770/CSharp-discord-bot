using Discord.Interactions;

namespace DiscordBot.Api.Modules;

public class EchoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("echo", "return your message")]
    public async Task Echo(string input)
    {
        Console.WriteLine("aboba");
        await RespondAsync(input, ephemeral: true);
    }
}