using DiscordBot.Bll.Entities;

namespace DiscordBot.Bll.Interfaces;

public interface IGuildSettingsRepository
{
    public Task<GuildSettingsEntity> Get(byte[] guildId, CancellationToken token);

    public Task SetCreateVoiceChannel(byte[] guildId, byte[] createVoiceChannelId, CancellationToken token);

    public Task Delete(byte[] guildId, CancellationToken token);
}