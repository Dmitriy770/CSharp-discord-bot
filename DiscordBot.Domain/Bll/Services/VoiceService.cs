using DiscordBot.Domain.Bll.Models;
using DiscordBot.Domain.Bll.Services.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Interfaces;

namespace DiscordBot.Domain.Bll.Services;

public class VoiceService : IVoiceService
{
    private readonly IVoiceUsersRepository _repository;
    private readonly ISessionRepository _sessionRepository;

    public VoiceService(IVoiceUsersRepository repository, ISessionRepository sessionRepository)
    {
        _repository = repository;
        _sessionRepository = sessionRepository;
    }

    public IEnumerable<ulong> SetVoiceLimit(ulong userId, int? limit)
    {
        if (limit is < 1 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        var voiceEntity = _repository.Get(userId) with { Limit = limit };
        _repository.SetOrUpdate(voiceEntity);

        return _sessionRepository.Get(userId);
    }

    public UserVoiceModel SetVoiceName(UserEntity user, string? name)
    {
        if (name?.Length is < 1 or > 20)
        {
            throw new ArgumentException("name length must be between 1 and 20");
        }

        var voiceEntity = _repository.Get(user.Id) with { Name = name };
        _repository.SetOrUpdate(voiceEntity);

        var voiceParams = _repository.Get(user.Id);
        return new UserVoiceModel(
            VoiceIDs: _sessionRepository.Get(user.Id),
            Name: voiceParams.Name ?? user.Name,
            Limit: voiceParams.Limit
        );
    }

    //TODO заменить входной параметр на userEntity
    public UserVoiceModel GetVoiceParams(ulong id)
    {
        var voiceEntity = _repository.Get(id);
        //TODO Добавить модель без массива с ID
        return new UserVoiceModel(
            VoiceIDs: Array.Empty<ulong>(),
            Name: voiceEntity.Name,
            Limit: voiceEntity.Limit
        );
    }

    //TODO убрать
    public IEnumerable<ulong> GetUserVoiceChannels(ulong userId)
    {
        return _sessionRepository.Get(userId);
    }

    public void SetUserVoice(ulong userId, ulong voiceId)
    {
        _sessionRepository.Set(userId, voiceId);
    }

    public void RemoveUserVoice(ulong voiceId)
    {
        _sessionRepository.Remove(voiceId);
    }
}