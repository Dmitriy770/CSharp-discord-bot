using DiscordBot.Dal.Entities;
using DiscordBot.Dal.Repositories.Interfaces;

namespace DiscordBot.Dal.Repositories;

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