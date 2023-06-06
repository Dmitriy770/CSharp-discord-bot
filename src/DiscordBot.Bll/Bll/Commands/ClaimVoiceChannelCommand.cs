using DiscordBot.Bll.Bll.Exceptions;
using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record ClaimVoiceChannelCommand(
    ulong GuildId,
    ulong NewOwnerId,
    VoiceChannelModel VoiceChannel
) : IRequest<VoiceChannelSettingsModel>;

public class ClaimVoiceChannelCommandHandler
    : IRequestHandler<ClaimVoiceChannelCommand, VoiceChannelSettingsModel>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;


    public ClaimVoiceChannelCommandHandler(
        IVoiceChannelCacheRepository voiceChannelCacheRepository,
        IVoiceChannelSettingsRepository voiceChannelSettingsRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
        _voiceChannelSettingsRepository = voiceChannelSettingsRepository;
    }

    public async Task<VoiceChannelSettingsModel> Handle(ClaimVoiceChannelCommand request, CancellationToken token)
    {
        var (guildId, newOwnerId, voiceChannel) = request;

        if (voiceChannel.Id is null)
        {
            throw new UserNotInVoiceException();
        }

        var voiceChannelCache = await _voiceChannelCacheRepository.Get(guildId, voiceChannel.Id.Value);

        if (voiceChannelCache.OwnerId == newOwnerId)
        {
            throw new VoiceClaimedException();
        }

        if (voiceChannelCache.OwnerId is not null)
        {
            throw new OwnerInVoiceException();
        }

        if (voiceChannel.UsersIds.All(user => user != newOwnerId))
        {
            throw new UserNotInVoiceException();
        }

        await _voiceChannelCacheRepository.Add(
            new VoiceChannelCacheEntity(
                guildId,
                voiceChannel.Id.Value,
                newOwnerId
            ));

        var settings = await _voiceChannelSettingsRepository.Get(
            BitConverter.GetBytes(guildId),
            BitConverter.GetBytes(newOwnerId),
            token);

        return new VoiceChannelSettingsModel(
            BitConverter.ToUInt64(settings.GuildId),
            BitConverter.ToUInt64(settings.UserId),
            settings.Name,
            settings.UsersLimit,
            settings.Bitrate
        );
    }
}