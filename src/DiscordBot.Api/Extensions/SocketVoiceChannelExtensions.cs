using Discord.WebSocket;
using DiscordBot.Bll.Models;

namespace DiscordBot.Api.Extensions;

public static class SocketVoiceChannelExtensions
{
    public static SocketVoiceChannel UpdateParams(
        this SocketVoiceChannel socketVoiceChannel,
        VoiceParamsModel voiceParams)
    {
        socketVoiceChannel.ModifyAsync(x =>
        {
            x.Name = voiceParams.Name;
            x.UserLimit = voiceParams.Limit;
        });
        return socketVoiceChannel;
    }
}