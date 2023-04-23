using Discord;
using Discord.WebSocket;
using DiscordBot.Bll.Models;

namespace DiscordBot.Api.Extensions;

public static class SocketVoiceChannelExtensions
{
    public static SocketVoiceChannel UpdateProperties(
        this SocketVoiceChannel channel,
        VoicePropertiesModel properties)
    {
        channel.ModifyAsync(o =>
        {
            o.Name = properties.Name ?? Optional<string>.Unspecified;
            o.UserLimit = properties.UserLimit ?? Optional<int?>.Unspecified;
        });
        return channel;
    }
}