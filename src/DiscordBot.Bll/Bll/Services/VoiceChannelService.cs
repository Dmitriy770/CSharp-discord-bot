using DiscordBot.Bll.Bll.Exceptions;
using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Bll.Services.Interfaces;
using DiscordBot.Bll.Interfaces;

namespace DiscordBot.Bll.Bll.Services;

public class VoiceChannelService : IVoiceChannelService
{
    private readonly IVoiceChannelRepository _repository;
    private readonly ISettingsRepository _settingsRepository;

    public VoiceChannelService(IVoiceChannelRepository repository, ISettingsRepository settingsRepository)
    {
        _repository = repository;
        _settingsRepository = settingsRepository;
    }

    public bool TryAdd(ulong guildId, ulong ownerId, ulong voiceChannelId)
    {
        if (_repository.TryGet(guildId, voiceChannelId, out _))
        {
            return false;
        }

        _repository.Add(guildId, ownerId, voiceChannelId);
        return true;
    }

    public VoiceChannelSettingsModel Update(VoiceChannelModel voiceChannel)
    {
        if (voiceChannel.UsersIds.All(id => id != voiceChannel.OwnerId))
        {
            throw new UserNotInVoiceException();
        }

        if (_repository.TryGetOwner(voiceChannel.GuildId, voiceChannel.Id, out var currentOwnerId) &&
            voiceChannel.UsersIds.Any(id => id == currentOwnerId))
        {
            throw new OwnerInVoiceException();
        }

        _repository.Add(voiceChannel.GuildId, voiceChannel.OwnerId, voiceChannel.Id);

        var settings = _settingsRepository.GetVoiceChannelSettings(voiceChannel.GuildId, voiceChannel.Id);
        return new VoiceChannelSettingsModel(
            GuildId: settings.GuildId,
            Id: settings.UserId,
            Name: settings.Name,
            Limit: settings.UserLimit,
            Bitrate: settings.Bitrate
        );
    }

    public void Delete(ulong guildId, ulong voiceChannelId)
    {
        _repository.Delete(guildId, voiceChannelId);
    }

    public bool TryGet(ulong guildId, ulong userId, out ulong voiceChannelId)
    {
        return _repository.TryGet(guildId, userId, out voiceChannelId);
    }
}