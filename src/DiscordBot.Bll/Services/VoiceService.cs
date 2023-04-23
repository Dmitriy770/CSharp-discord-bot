using Discord;
using DiscordBot.Bll.Exceptions;
using DiscordBot.Bll.Services.Interfaces;
using DiscordBot.Dal.Entities;
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

    public void ClaimVoice(ulong userId, ulong? voiceId, IEnumerable<ulong> membersIds)
    {
        if (voiceId == null)
        {
            throw new UserNotInVoiceException();
        }
    
        var ownerId = _sessionRepository.GetOwner(voiceId.Value);

        if (userId == ownerId)
        {
            throw new VoiceClaimedException();
        }
    
        if (membersIds.Any(x => x == ownerId))
        {
            throw new OwnerInVoiceException();
        }
    
        _sessionRepository.Set(userId, voiceId.Value);
    }

    public void SetOrUpdateProperties(ulong userId, VoiceChannelProperties properties)
    {
        var oldProperties = _repository.Get(userId);

        var voiceEntity = new VoiceUserEntity(
            Id: userId,
            Name: properties.Name.GetValueOrDefault(oldProperties.Name),
            UserLimit: properties.UserLimit.GetValueOrDefault(oldProperties.UserLimit)
        );
        
        _repository.SetOrUpdate(voiceEntity);
    }

    public VoiceChannelProperties GetProperties(ulong userId)
    {
        var properties = _repository.Get(userId);

        return new VoiceChannelProperties
        {
            Name = properties.Name ?? Optional<string>.Unspecified,
            UserLimit = properties.UserLimit
        };
    }

    public void SetUserVoice(ulong userId, ulong voiceId)
    {
        _sessionRepository.Set(userId, voiceId);
    }

    public void RemoveUserVoice(ulong voiceId)
    {
        _sessionRepository.Remove(voiceId);
    }

    public ulong? GetUserVoice(ulong userId)
    {
        return _sessionRepository.Get(userId);
    }
}
