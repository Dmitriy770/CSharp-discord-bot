namespace DiscordBot.Bll.Interfaces;

public interface IVoiceChannelRepository
{
    public void Add(ulong guildId, ulong voiceChannelOwnerId, ulong voiceChannelId);

    public void Delete(ulong guildId, ulong voiceChannelId);

    public ulong Get(ulong guildId, ulong userId);
}