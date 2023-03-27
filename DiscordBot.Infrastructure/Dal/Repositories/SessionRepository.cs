using System.Dynamic;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Interfaces;

namespace DiscordBot.Infrastructure.Dal.Repositories;

internal class SessionRepository : ISessionRepository
{
    private readonly Dictionary<ulong, ulong> _sessionData = new();

    public IEnumerable<ulong> Get(ulong userId)
    {
        var userVoiceChannels = new List<ulong>();

        foreach (var data in _sessionData)
        {
            if (data.Value == userId)
            {
                userVoiceChannels.Add(data.Key);
            }
        }

        return userVoiceChannels;
    }

    public void Set(ulong userId, ulong voiceId)
    {
        _sessionData[voiceId] = userId;
    }

    public void Remove(ulong voiceId)
    {
        if (_sessionData.ContainsKey(voiceId))
        {
            _sessionData.Remove(voiceId);
        }
    }
}