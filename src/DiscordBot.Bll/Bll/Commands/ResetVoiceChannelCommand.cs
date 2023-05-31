using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record ResetVoiceChannelCommand(
    ulong GuildId,
    ulong UserId,
    ulong? CurrentUserChannelId
) : IRequest<UpdateSettingsResult>;

public class ResetVoiceChannelCommandHandler
    : IRequestHandler<ResetVoiceChannelCommand, UpdateSettingsResult>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;

    public ResetVoiceChannelCommandHandler(
        IVoiceChannelCacheRepository voiceChannelCacheRepository,
        IVoiceChannelSettingsRepository voiceChannelSettingsRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
        _voiceChannelSettingsRepository = voiceChannelSettingsRepository;
    }

    public async Task<UpdateSettingsResult> Handle(ResetVoiceChannelCommand request, CancellationToken token)
    {
        var (guildId, userId, voiceChannelId) = request;

        await _voiceChannelSettingsRepository.Delete(
            BitConverter.GetBytes(guildId),
            BitConverter.GetBytes(userId),
            token);

        if (voiceChannelId is not null)
        {
            var voiceChannelCache = await _voiceChannelCacheRepository.Get(guildId, voiceChannelId.Value);

            if (voiceChannelCache.OwnerId != userId)
            {
                voiceChannelId = null;
            }
        }

        return new UpdateSettingsResult(
            voiceChannelId,
            new VoiceChannelSettingsModel(
                guildId,
                userId,
                null,
                null,
                null
            )
        );
    }
}