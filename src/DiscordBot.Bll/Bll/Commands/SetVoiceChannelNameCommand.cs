using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record SetVoiceChannelNameCommand(
    ulong GuildId,
    ulong UserId,
    ulong? CurrentUserChannelId,
    string? VoiceChannelName
) : IRequest<UpdateSettingsResult>;

public class SetVoiceChannelNameCommandHandler
    : IRequestHandler<SetVoiceChannelNameCommand, UpdateSettingsResult>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;

    public SetVoiceChannelNameCommandHandler(
        IVoiceChannelCacheRepository voiceChannelCacheRepository,
        IVoiceChannelSettingsRepository voiceChannelSettingsRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
        _voiceChannelSettingsRepository = voiceChannelSettingsRepository;
    }

    public async Task<UpdateSettingsResult> Handle(SetVoiceChannelNameCommand request, CancellationToken token)
    {
        var (guildId, userId, voiceChannelId, name) = request;

        if (name?.Length is <= 3 or >= 15)
        {
            throw new ArgumentException("Длина названия голосового канала должна быть в пределе от 3 до 15",
                nameof(request.VoiceChannelName));
        }

        var settings = await _voiceChannelSettingsRepository.SetName(
            BitConverter.GetBytes(guildId),
            BitConverter.GetBytes(userId),
            name,
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