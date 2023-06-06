using DiscordBot.VoiceManager.Domain.Bll.Models;
using DiscordBot.VoiceManager.Domain.Interfaces;
using MediatR;

namespace DiscordBot.VoiceManager.Domain.Bll.Queries;

public record GetVoiceChannelQuery(
    ulong GuildId,
    ulong UserId
) : IRequest<VoiceChannelSettingsModel>;

public class GetVoiceChannelQueryHandler
    : IRequestHandler<GetVoiceChannelQuery, VoiceChannelSettingsModel>
{
    private readonly IVoiceChannelSettingsRepository _voiceChannelSettingsRepository;

    public GetVoiceChannelQueryHandler(IVoiceChannelSettingsRepository voiceChannelSettingsRepository)
    {
        _voiceChannelSettingsRepository = voiceChannelSettingsRepository;
    }

    public async Task<VoiceChannelSettingsModel> Handle(GetVoiceChannelQuery request, CancellationToken token)
    {
        var (guildId, userId) = request;

        var settings = await _voiceChannelSettingsRepository.Get(
            BitConverter.GetBytes(guildId),
            BitConverter.GetBytes(userId),
            token
        );

        return new VoiceChannelSettingsModel(
            BitConverter.ToUInt64(settings.GuildId),
            BitConverter.ToUInt64(settings.UserId),
            settings.Name,
            settings.UsersLimit,
            settings.Bitrate
        );
    }
}