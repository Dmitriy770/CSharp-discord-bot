using System.Transactions;
using DiscordBot.Dal.Repositories.Interfaces;
using DiscordBot.Dal.Settings;
using Npgsql;

namespace DiscordBot.Dal.Repositories;

public abstract class BaseDbRepository : IDbRepository
{
    private readonly DatabaseOptions _databaseSettings;

    protected BaseDbRepository(DatabaseOptions databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    protected async Task<NpgsqlConnection> GetAndOpenConnection()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = _databaseSettings.Host,
            Port = _databaseSettings.Port,
            Username = _databaseSettings.User,
            Password = _databaseSettings.Password,
            Database = _databaseSettings.Database,
            Pooling = _databaseSettings.Pooling
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