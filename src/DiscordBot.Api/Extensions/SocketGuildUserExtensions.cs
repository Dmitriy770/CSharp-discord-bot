using Discord.WebSocket;

namespace DiscordBot.Api.Extensions;

public static class SocketGuildUserExtensions
{
    public static async Task Move(this SocketGuildUser user, ulong newChannelId)
    {
        await user.ModifyAsync(x => { x.ChannelId = newChannelId; });
    }
}