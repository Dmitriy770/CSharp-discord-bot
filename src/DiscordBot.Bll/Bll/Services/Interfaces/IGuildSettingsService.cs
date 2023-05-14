using DiscordBot.Bll.Bll.Models;

namespace DiscordBot.Bll.Bll.Services.Interfaces;

public interface IGuildSettingsService
{
    public void Set(GuildSettingsModel settings);

    public GuildSettingsModel Get(ulong guildId);
}