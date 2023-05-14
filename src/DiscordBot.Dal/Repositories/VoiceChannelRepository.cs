using DiscordBot.Bll.Interfaces;

namespace DiscordBot.Dal.Repositories;

public class VoiceChannelRepository : IVoiceChannelRepository
{
    public void Add(ulong guildId, ulong ownerId, ulong voiceChannelId)
    {
        throw new NotImplementedException();
    }

    public void Delete(ulong guildId, ulong voiceChannelId)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(ulong guildId, ulong userId, out ulong channelId)
    {
        throw new NotImplementedException();
    }

    public bool TryGetOwner(ulong guildId, ulong voiceChannelId, out ulong ownerId)
    {
        throw new NotImplementedException();
    }
}