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

    //TODO создать модель VoiceModel с id и участниками, текущую VoiceModel переименовать в VoiceParamsModel
    public UpdateVoicesModel ClaimVoice(UserModel user, ulong? voiceId, IEnumerable<ulong> userIDs)
    {
        if (voiceId == null)
        {
            throw new ArgumentException("User not in voice");
        }

        var ownerId = _sessionRepository.GetOwner(voiceId.Value);

        if (userIDs.Any(x => x == ownerId))
        {
            //TODO добавить кастомные ошибки
            throw new ArgumentException("Owner in voice");
        }

        _sessionRepository.Set(user.Id, voiceId.Value);

        return new UpdateVoicesModel(
            Params: GetVoiceParams(user),
            VoiceIDs: new[] { voiceId.Value }
        );
    }

    public UpdateVoicesModel SetVoiceLimit(UserModel user, int? limit)
    {
        if (limit is < 1 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        var voiceEntity = _repository.Get(user.Id) with { Limit = limit };
        _repository.SetOrUpdate(voiceEntity);

        return new UpdateVoicesModel(
            Params: GetVoiceParams(user),
            VoiceIDs: _sessionRepository.Get(user.Id)
        );
    }

    public UpdateVoicesModel SetVoiceName(UserModel user, string? name)
    {
        if (name?.Length is < 1 or > 20)
        {
            throw new ArgumentException("Name length must be between 1 and 20");
        }

        var voiceEntity = _repository.Get(user.Id) with { Name = name };
        _repository.SetOrUpdate(voiceEntity);

        return new UpdateVoicesModel(
            Params: GetVoiceParams(user),
            VoiceIDs: _sessionRepository.Get(user.Id)
        );
    }

    public VoiceModel GetVoiceParams(UserModel user)
    {
        var voiceEntity = _repository.Get(user.Id);
        return new VoiceModel(
            Name: voiceEntity.Name ?? $"{user.Name}'s channel",
            Limit: voiceEntity.Limit
        );
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