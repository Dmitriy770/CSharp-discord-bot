using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record DeleteVoiceChannelOwnerCommand(
    ulong GuildId,
    ulong UserId,
    ulong VoiceChannelId
) : IRequest;

public class DeleteVoiceChannelOwnerCommandHandler
    : IRequestHandler<DeleteVoiceChannelOwnerCommand>
{
    private readonly IVoiceChannelCacheRepository _voiceChannelCacheRepository;

    public DeleteVoiceChannelOwnerCommandHandler(IVoiceChannelCacheRepository voiceChannelCacheRepository)
    {
        _voiceChannelCacheRepository = voiceChannelCacheRepository;
    }

    public async Task Handle(DeleteVoiceChannelOwnerCommand request, CancellationToken token)
    {
        var voiceChannelCache = await _voiceChannelCacheRepository.Get(
            request.GuildId,
            request.VoiceChannelId);

        if (voiceChannelCache.OwnerId == request.UserId)
        {
            await _voiceChannelCacheRepository.Delete(
                request.GuildId,
                request.VoiceChannelId);
        }
    }
}