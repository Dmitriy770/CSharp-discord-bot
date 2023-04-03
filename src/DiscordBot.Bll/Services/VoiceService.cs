using DiscordBot.Bll.Exceptions;
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
            throw new UserNotInVoiceException();
        }

        var ownerId = _sessionRepository.GetOwner(voiceModel.Id);

        if (user.Id == ownerId)
        {
            throw new VoiceClaimedException();
        }

        if (voiceModel.UserIDs.Any(x => x == ownerId))
        {
            throw new OwnerInVoiceException();
        }

        _sessionRepository.Set(user.Id, voiceModel.Id);

        return new UpdateVoicesModel(
            Params: GetVoiceParams(user),
            VoiceIDs: new[] { voiceModel.Id }
        );
    }

    public UpdateVoicesModel SetVoiceLimit(UserModel user, byte? limit)
    {
        if (limit is < 1 or > 99)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "limit must be between 1 and 99");
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
            throw new ArgumentOutOfRangeException(nameof(name), "Name length must be between 1 and 20");
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
