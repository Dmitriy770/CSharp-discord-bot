using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Commands;

public record SetCreateVoiceChannelCommand(
    ulong GuildId,
    ulong? CreateVoiceChannelId
) : IRequest;

public class SetCreateVoiceChannelCommandHandler
    : IRequestHandler<SetCreateVoiceChannelCommand>
{
    private readonly IGuildSettingsRepository _guildSettingsRepository;

    public SetCreateVoiceChannelCommandHandler(
        IGuildSettingsRepository guildSettingsRepository)
    {
        _guildSettingsRepository = guildSettingsRepository;
    }

    public async Task Handle(SetCreateVoiceChannelCommand request, CancellationToken token)
    {
        var (guildId, createVoiceChannelId) = request;

        await _guildSettingsRepository.SetCreateVoiceChannel(
            BitConverter.GetBytes(guildId),
            createVoiceChannelId is null ? null : BitConverter.GetBytes(createVoiceChannelId.Value),
            token
        );
    }
}