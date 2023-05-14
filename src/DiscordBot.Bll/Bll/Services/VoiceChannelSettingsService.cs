using DiscordBot.Bll.Bll.Extensions;
using DiscordBot.Bll.Bll.Models;
using DiscordBot.Bll.Bll.Services.Interfaces;
using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;

namespace DiscordBot.Bll.Bll.Services;

public class VoiceChannelSettingsService : IVoiceChannelSettingsService
{
    private readonly ISettingsRepository _repository;

    public VoiceChannelSettingsService(ISettingsRepository repository)
    {
        _repository = repository;
    }

    public void Set(VoiceChannelSettingsModel settings)
    {
        settings.IsValid();

        _repository.SetVoiceChannelSettings(
            new VoiceChannelSettingsEntity(
                GuildId: settings.GuildId,
                Id: settings.Id,
                Name: settings.Name,
                Limit: settings.Limit,
                Bitrate: settings.Bitrate
            ));
    }

    public VoiceChannelSettingsModel Get(ulong guildId, ulong userId)
    {
        var settings = _repository.GetVoiceChannelSettings(guildId, userId);

        return new VoiceChannelSettingsModel(
            GuildId: settings.GuildId,
            Id: settings.Id,
            Name: settings.Name,
            Limit: settings.Limit,
            Bitrate: settings.Bitrate
        );
    }
}