using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record SetVoiceChannelOwnerCommand(
    ulong GuildId,
    ulong UserId,
    ulong VoiceChannelId
) : IRequest;

public class SetVoiceChannelOwnerCommandHandler
    : IRequestHandler<SetVoiceChannelOwnerCommand>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;

    public SetVoiceChannelOwnerCommandHandler(IVoiceChannelCacheRepository voiceChannelCacheRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
    }

    public async Task Handle(SetVoiceChannelOwnerCommand request, CancellationToken token)
    {
        await _voiceChannelCacheRepository.Add(
            new VoiceChannelCacheEntity(
                request.GuildId,
                request.VoiceChannelId,
                request.UserId
            ));
    }
}