using DiscordBot.VoiceManager.Domain.Entities;

namespace DiscordBot.VoiceManager.Domain.Interfaces;

public interface IVoiceChannelSettingsRepository
{
    public Task<VoiceChannelSettingsEntity> Get(byte[] guildId, byte[] userId, CancellationToken token);

    public Task<VoiceChannelSettingsEntity> SetName(byte[] guildId, byte[] userId, string? name, CancellationToken token);

    public Task<VoiceChannelSettingsEntity> SetUsersLimit(byte[] guildId, byte[] userId, int? usersLimit, CancellationToken token);

    public Task<VoiceChannelSettingsEntity> SetBitrate(byte[] guildId, byte[] userId, int? bitrate, CancellationToken token);

    public Task Delete(byte[] guildId, byte[] userId, CancellationToken token);
}