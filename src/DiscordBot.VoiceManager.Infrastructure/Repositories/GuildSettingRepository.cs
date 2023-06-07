using Dapper;
using DiscordBot.VoiceManager.Domain.Entities;
using DiscordBot.VoiceManager.Domain.Interfaces;
using DiscordBot.VoiceManager.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace DiscordBot.VoiceManager.Infrastructure.Repositories;

public class GuildSettingRepository : BaseDbRepository, IGuildSettingsRepository
{
    public GuildSettingRepository(IOptions<DbOptions> dalSettings) : base(dalSettings.Value)
    {
    }


    public async Task<GuildSettingsEntity> Get(byte[] guildId, CancellationToken token)
    {
        const string sqlQuery = @"
select id
     , create_voice_channel_id
from guilds_settings
where id=@Id;  
";

        var sqlQueryParams = new
        {
            Id = guildId
        };

        await using var connection = await GetAndOpenConnection();
        var guildSettings = await connection.QueryAsync<GuildSettingsEntity>(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));

        return guildSettings.FirstOrDefault(
            new GuildSettingsEntity
            {
                Id = guildId,
                CreateVoiceChannelId = null
            });
    }

    public async Task SetCreateVoiceChannel(byte[] guildId, byte[] createVoiceChannelId, CancellationToken token)
    {
        const string sqlExecute = @"
insert into guilds_settings (id, create_voice_channel_id) 
    values (@Id, @CreateVoiceChannelId)
    on conflict (id) do update set create_voice_channel_id = @CreateVoiceChannelId;
";

        var sqlExecuteParams = new
        {
            Id = guildId,
            CreateVoiceChannelId = createVoiceChannelId
        };

        await using var connection = await GetAndOpenConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlExecute,
                sqlExecuteParams,
                cancellationToken: token));
    }

    public async Task Delete(byte[] guildId, CancellationToken token)
    {
        const string sqlExecute = @"
delete from guilds_settings
    where id=@Id;
";

        var sqlExecuteParams = new
        {
            Id = guildId
        };

        await using var connection = await GetAndOpenConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlExecute,
                sqlExecuteParams,
                cancellationToken: token));
    }
}