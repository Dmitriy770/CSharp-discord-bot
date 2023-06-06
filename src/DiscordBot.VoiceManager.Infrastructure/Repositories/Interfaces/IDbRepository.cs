using System.Transactions;

namespace DiscordBot.VoiceManager.Infrastructure.Repositories.Interfaces;

public interface IDbRepository
{
    TransactionScope CreateTransactionScope(IsolationLevel level = IsolationLevel.ReadCommitted);
}