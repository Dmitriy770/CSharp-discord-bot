using Dapper;
using DiscordBot.Bll.Entities;
using DiscordBot.Bll.Interfaces;
using DiscordBot.Dal.Settings;
using Microsoft.Extensions.Options;

namespace DiscordBot.Dal.Repositories;

public class VoiceChannelSettingsRepository : BaseRepository, IVoiceChannelSettingsRepository
{
    public VoiceChannelSettingsRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<VoiceChannelSettingsEntity> Get(byte[] guildId, byte[] userId, CancellationToken token)
    {
        const string sqlQuery = @"
select guild_id
     , user_id
     , name
     , users_limit
     , bitrate
from voice_channels_settings
where guild_id=@GuildId and user_id=@UserId;
";

        var sqlQueryParams = new
        {
            GuildId = guildId,
            UserId = userId
        };

        await using var connection = await GetAndOpenConnection();
        var channelSettings = await connection.QueryAsync<VoiceChannelSettingsEntity>(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));

        return channelSettings.FirstOrDefault(
            new VoiceChannelSettingsEntity
            {
                GuildId = guildId,
                UserId = userId
            });
    }

    public async Task<VoiceChannelSettingsEntity> SetName(byte[] guildId, byte[] userId, string? name,
        CancellationToken token)
    {
        const string sqlQuery = @"
insert into voice_channels_settings  (guild_id, user_id, name)
    values (@GuildId, @UserId, @Name)
    on conflict (guild_id, user_id) do update set name = @Name
returning guild_id, user_id, name, users_limit, bitrate;
";

        var sqlQueryParams = new
        {
            GuildId = guildId,
            UserId = userId,
            Name = name
        };

        await using var connection = await GetAndOpenConnection();
        var settings = await connection.QueryAsync<VoiceChannelSettingsEntity>(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));

        return settings.First();
    }

    public async Task<VoiceChannelSettingsEntity> SetUsersLimit(byte[] guildId, byte[] userId, int? usersLimit,
        CancellationToken token)
    {
        const string sqlQuery = @"
insert into voice_channels_settings (guild_id, user_id, users_limit)
    values (@GuildId, @UserId, @UsersLimit)
    on conflict (guild_id, user_id) do update set users_limit = @UsersLimit
returning guild_id, user_id, name, users_limit, bitrate;
";

        var sqlQueryParams = new
        {
            GuildId = guildId,
            UserId = userId,
            UsersLimit = usersLimit
        };

        await using var connection = await GetAndOpenConnection();
        var settings = await connection.QueryAsync<VoiceChannelSettingsEntity>(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));

        return settings.First();
    }

    public async Task<VoiceChannelSettingsEntity> SetBitrate(byte[] guildId, byte[] userId, int? bitrate,
        CancellationToken token)
    {
        const string sqlQuery = @"
insert into voice_channels_settings (guild_id, user_id, bitrate)
    values (@GuildId, @UserId, @Bitrate)
    on conflict (guild_id, user_id) do update set bitrate = @Bitrate
returning guild_id, user_id, name, users_limit, bitrate;
";

        var sqlQueryParams = new
        {
            GuildId = guildId,
            UserId = userId,
            Bitrate = bitrate
        };

        await using var connection = await GetAndOpenConnection();
        var settings = await connection.QueryAsync<VoiceChannelSettingsEntity>(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));

        return settings.First();
    }

    public async Task Delete(byte[] guildId, byte[] userId, CancellationToken token)
    {
        const string sqlExecute = @"
delete from voice_channels_settings
    where guild_id=@GuildId and user_id=@UserId;
";

        var sqlExecuteParams = new
        {
            GuildId = guildId,
            UserId = userId
        };

        await using var connection = await GetAndOpenConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlExecute,
                sqlExecuteParams,
                cancellationToken: token));
    }
}