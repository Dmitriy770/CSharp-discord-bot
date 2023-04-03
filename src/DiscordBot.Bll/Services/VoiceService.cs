using DiscordBot.Bll.Models;
using DiscordBot.Bll.Services.Interfaces;
using DiscordBot.Dal.Repositories.Interfaces;

namespace DiscordBot.Bll.Services;

public class VoiceService : IVoiceService
{
    private readonly IVoiceUsersRepository _repository;
    private readonly ISessionRepository _sessionRepository;

    public VoiceService(IVoiceUsersRepository repository, ISessionRepository sessionRepository)
    {
        _repository = repository;
        _sessionRepository = sessionRepository;
    }

    public UpdateVoicesModel ClaimVoice(UserModel user, VoiceModel? voiceModel)
    {
        if (voiceModel == null)
        {
            throw new ArgumentException("User not in voice", nameof(voiceModel));
        }

        var ownerId = _sessionRepository.GetOwner(voiceModel.Id);

        if (user.Id == ownerId)
        {
            throw new ArgumentException("You're owner of this channel", nameof(voiceModel.Id));
        }

        if (voiceModel.UserIDs.Any(x => x == ownerId))
        {
            //TODO добавить кастомные ошибки
            throw new ArgumentException("Owner in voice", nameof(voiceModel.Id));
        }

        _sessionRepository.Set(user.Id, voiceModel.Id);

        return new UpdateVoicesModel(
            Params: GetVoiceParams(user),
            VoiceIDs: new[] { voiceModel.Id }
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

    public VoiceParamsModel GetVoiceParams(UserModel user)
    {
        var voiceEntity = _repository.Get(user.Id);
        return new VoiceParamsModel(
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
