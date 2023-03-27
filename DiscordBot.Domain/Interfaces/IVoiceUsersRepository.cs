using DiscordBot.Domain.Entities;

namespace DiscordBot.Domain.Interfaces;

public interface IVoiceUsersRepository
{
    void SetOrUpdate(VoiceUserEntity voiceUser);

    VoiceUserEntity Get(ulong id);
}