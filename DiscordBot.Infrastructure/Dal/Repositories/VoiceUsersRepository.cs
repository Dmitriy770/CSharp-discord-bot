using System.Dynamic;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Interfaces;

namespace DiscordBot.Infrastructure.Dal.Repositories;

internal class VoiceUsersRepository : IVoiceUsersRepository
{
    private readonly Dictionary<ulong, VoiceUserEntity> _voiceUsers = new();

    public void SetOrUpdate(VoiceUserEntity voiceUser)
    {
        _voiceUsers[voiceUser.Id] = voiceUser;
    }

    public VoiceUserEntity Get(ulong id)
    {
        if (_voiceUsers.ContainsKey(id))
        {
            return _voiceUsers[id];
        }

        return new VoiceUserEntity(id, null, null);
    }
}