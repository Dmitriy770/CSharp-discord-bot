namespace DiscordBot.Dal.Repositories.Interfaces;

public interface ISessionRepository
{
    public ulong? GetOwner(ulong voiceId);
    
    public IEnumerable<ulong> Get(ulong userId);

    public void Set(ulong userId, ulong voiceId);

    public void Remove(ulong voiceId);
}