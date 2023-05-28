using System.Transactions;
using DiscordBot.Dal.Repositories.Interfaces;
using DiscordBot.Dal.Settings;
using Npgsql;

namespace DiscordBot.Dal.Repositories;

public abstract class BaseRepository : IDbRepository
{
    private readonly DalOptions _dalSettings;

    protected BaseRepository(DalOptions dalSettings)
    {
        _dalSettings = dalSettings;
    }

    protected async Task<NpgsqlConnection> GetAndOpenConnection()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = _dalSettings.Host,
            Port = _dalSettings.Port,
            Username = _dalSettings.User,
            Password = _dalSettings.Password,
            Database = _dalSettings.Database,
            Pooling = _dalSettings.Pooling
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