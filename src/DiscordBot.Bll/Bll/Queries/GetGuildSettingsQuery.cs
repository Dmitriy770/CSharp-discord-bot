using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Interfaces;
using MediatR;

namespace DiscordBot.Bll.Bll.Queries;

public record GetGuildSettingsQuery(
    ulong GuildId
) : IRequest<GuildSettingsModel>;

public class GetGuildSettingsQueryHandle
    : IRequestHandler<GetGuildSettingsQuery, GuildSettingsModel>
{
    private readonly IGuildSettingsRepository _guildSettingsRepository;

    public GetGuildSettingsQueryHandle(IGuildSettingsRepository guildSettingsRepository)
    {
        _guildSettingsRepository = guildSettingsRepository;
    }

    public async Task<GuildSettingsModel> Handle(GetGuildSettingsQuery request, CancellationToken token)
    {
        var settings = await _guildSettingsRepository.Get(
            BitConverter.GetBytes(request.GuildId),
            token);

        return new GuildSettingsModel(
            BitConverter.ToUInt64(settings.Id),
            BitConverter.ToUInt64(settings.CreateVoiceChannelId)
        );
    }
}