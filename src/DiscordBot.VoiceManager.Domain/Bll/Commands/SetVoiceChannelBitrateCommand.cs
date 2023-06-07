using DiscordBot.VoiceManager.Domain.Bll.Models;
using DiscordBot.VoiceManager.Domain.Interfaces;
using MediatR;

namespace DiscordBot.VoiceManager.Domain.Bll.Commands;

public record SetVoiceChannelBitrateCommand(
    ulong GuildId,
    ulong UserId,
    ulong? CurrentUserChannelId,
    int? VoiceChannelBitrate
) : IRequest<UpdateSettingsResult>;

public class SetVoiceChannelBitrateCommandHandler
    : IRequestHandler<SetVoiceChannelBitrateCommand, UpdateSettingsResult>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;
    private const int KILO = 1000;

    public SetVoiceChannelBitrateCommandHandler(
        IVoiceChannelCacheRepository voiceChannelCacheRepository,
        IVoiceChannelSettingsRepository voiceChannelSettingsRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
        _voiceChannelSettingsRepository = voiceChannelSettingsRepository;
    }

    public async Task<UpdateSettingsResult> Handle(SetVoiceChannelBitrateCommand request, CancellationToken token)
    {
        var (guildId, userId, voiceChannelId, bitrate) = request;

        if (bitrate is < 8 or > 384)
        {
            throw new ArgumentException(
                "Битрейт должен быть в пределе от 8 до 384", nameof(request.VoiceChannelBitrate));
        }

        var settings = await _voiceChannelSettingsRepository.SetBitrate(
            BitConverter.GetBytes(guildId),
            BitConverter.GetBytes(userId),
            bitrate * KILO,
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