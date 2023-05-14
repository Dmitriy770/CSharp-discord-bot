namespace DiscordBot.Bll.Interfaces;

public interface IVoiceChannelRepository
{
    public void Add(ulong guildId, ulong ownerId, ulong voiceChannelId);

    public void Delete(ulong guildId, ulong voiceChannelId);

    public bool TryGet(ulong guildId, ulong userId, out ulong channelId);

    public bool TryGetOwner(ulong guildId, ulong voiceChannelId, out ulong ownerId);
}