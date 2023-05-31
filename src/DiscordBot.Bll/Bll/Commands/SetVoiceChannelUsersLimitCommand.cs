using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record SetVoiceChannelUsersLimitCommand(
    ulong GuildId,
    ulong UserId,
    ulong? CurrentUserChannelId,
    int? VoiceChannelLimit
) : IRequest<UpdateSettingsResult>;

public class SetVoiceChannelUsersLimitCommandHandler
    : IRequestHandler<SetVoiceChannelUsersLimitCommand, UpdateSettingsResult>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;

    public SetVoiceChannelUsersLimitCommandHandler(
        IVoiceChannelCacheRepository voiceChannelCacheRepository,
        IVoiceChannelSettingsRepository voiceChannelSettingsRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
        _voiceChannelSettingsRepository = voiceChannelSettingsRepository;
    }

    public async Task<UpdateSettingsResult> Handle(SetVoiceChannelUsersLimitCommand request, CancellationToken token)
    {
        var (guildId, userId, voiceChannelId, limit) = request;

        if (limit is < 1 or > 99)
        {
            throw new ArgumentException(
                "Лимит пользователей должен быть в пределе от 1 до 99", nameof(request.VoiceChannelLimit));
        }

        var settings = await _voiceChannelSettingsRepository.SetUsersLimit(
            BitConverter.GetBytes(guildId),
            BitConverter.GetBytes(userId),
            limit,
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
                BitConverter.ToUInt64(settings.GuildId),
                BitConverter.ToUInt64(settings.UserId),
                settings.Name,
                settings.UsersLimit,
                settings.Bitrate
            )
        );
    }
}