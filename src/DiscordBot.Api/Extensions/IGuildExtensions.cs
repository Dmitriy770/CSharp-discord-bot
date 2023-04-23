using Discord;

namespace DiscordBot.Api.Extensions;

public static class IGuildExtensions
{
    public static async Task<IVoiceChannel> CreateUserVoice(this IGuild guild, VoiceChannelProperties properties)
    {
        return await guild.CreateVoiceChannelAsync(properties.Name.Value, o =>
        {
            o.CategoryId = properties.CategoryId;
            o.Name = properties.Name;
            o.UserLimit = properties.UserLimit;
        });
    }
}