namespace DiscordBot.Bll.Exceptions;

public class UserNotInVoiceException : Exception
{
    public UserNotInVoiceException() : base("You're not in a voice channel")
    {
        
    }
}