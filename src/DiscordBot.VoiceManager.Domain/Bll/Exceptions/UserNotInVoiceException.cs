namespace DiscordBot.VoiceManager.Domain.Bll.Exceptions;

public class UserNotInVoiceException : Exception
{
    public UserNotInVoiceException() : base("You're not in a voice channel")
    {
        
    }
}