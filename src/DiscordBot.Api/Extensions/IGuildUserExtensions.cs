using Discord;

namespace DiscordBot.Api.Extensions;

public static class IGuildUserExtensions
{
    public static async Task MoveAsync(this IGuildUser user, ulong newChannelId)
    {
        await user.ModifyAsync(x => { x.ChannelId = newChannelId; });
    }
}