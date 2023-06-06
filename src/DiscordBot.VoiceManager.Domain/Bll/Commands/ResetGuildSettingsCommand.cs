using DiscordBot.VoiceManager.Domain.Interfaces;
using MediatR;

namespace DiscordBot.VoiceManager.Domain.Bll.Commands;

public record ResetGuildSettingsCommand(
    ulong GuildId
) : IRequest;

public class ResetGuildSettingsCommandHandler
    : IRequestHandler<ResetGuildSettingsCommand>
{
    private readonly IGuildSettingsRepository _guildSettingsRepository;

    public ResetGuildSettingsCommandHandler(IGuildSettingsRepository guildSettingsRepository)
    {
        _guildSettingsRepository = guildSettingsRepository;
    }

    public async Task Handle(ResetGuildSettingsCommand request, CancellationToken token)
    {
        await _guildSettingsRepository.Delete(
            BitConverter.GetBytes(request.GuildId),
            token);
    }
}