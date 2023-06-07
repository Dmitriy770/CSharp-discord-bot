using System.Transactions;
using DiscordBot.VoiceManager.Infrastructure.Repositories.Interfaces;
using DiscordBot.VoiceManager.Infrastructure.Settings;
using Npgsql;

namespace DiscordBot.VoiceManager.Infrastructure.Repositories;

public abstract class BaseDbRepository : IDbRepository
{
    private readonly DbOptions _dbSettings;

    protected BaseDbRepository(DbOptions dbSettings)
    {
        _dbSettings = dbSettings;
    }

    protected async Task<NpgsqlConnection> GetAndOpenConnection()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = _dbSettings.Host,
            Port = _dbSettings.Port,
            Username = _dbSettings.User,
            Password = _dbSettings.Password,
            Database = _dbSettings.Database,
            Pooling = _dbSettings.Pooling
        };
        
        var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();
        await connection.ReloadTypesAsync();

        return connection;
    }

    public TransactionScope CreateTransactionScope(IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TimeSpan.FromSeconds(5)
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}