﻿using Discord;

namespace DiscordBot.Api.Extensions;

public static class IGuildUserExtensions
{
    public static async Task MoveAsync(this IGuildUser user, IVoiceChannel newVoiceChannel)
    {
        await user.ModifyAsync(x => { x.ChannelId = newVoiceChannel.Id; });
    }
}