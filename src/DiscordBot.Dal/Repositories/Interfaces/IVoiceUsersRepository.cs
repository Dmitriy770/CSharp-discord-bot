using DiscordBot.Dal.Entities;

namespace DiscordBot.Dal.Repositories.Interfaces;

public interface IVoiceUsersRepository
{
    void SetOrUpdate(VoiceUserEntity voiceUser);

    VoiceUserEntity Get(ulong id);
}