using Discord;

namespace DiscordBot.Api.Extensions;

public static class IVoiceChannelExtensions
{
    public static async Task<IVoiceChannel> UpdateAsync(
        this IVoiceChannel channel,
        VoiceChannelProperties properties)
    {
        await channel.ModifyAsync(o =>
        {
            o.CategoryId = properties.CategoryId;
            o.Name = properties.Name;
            o.UserLimit = properties.UserLimit;
        });
        return channel;
    }
}