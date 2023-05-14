using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Bll.Services.Interfaces;
using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;

namespace DiscordBot.Bll.Bll.Services;

public class GuildSettingsService : IGuildSettingsService
{
    private readonly ISettingsRepository _repository;

    public GuildSettingsService(ISettingsRepository repository)
    {
        _repository = repository;
    }

    public void Set(GuildSettingsModel settings)
    {
        _repository.SetSettings(
            new GuildSettingsEntity(
                Id: settings.Id,
                CreateVoiceChannelId: settings.CreateVoiceChannelId
            ));
    }

    public GuildSettingsModel Get(ulong guildId)
    {
        var settings = _repository.GetSettings(guildId);

        return new GuildSettingsModel(
            Id: settings.Id,
            CreateVoiceChannelId: settings.CreateVoiceChannelId
        );
    }
}