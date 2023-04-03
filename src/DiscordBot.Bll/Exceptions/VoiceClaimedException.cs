namespace DiscordBot.Bll.Exceptions;

public class VoiceClaimedException : Exception
{
    public VoiceClaimedException() : base("The voice channel is already yours")
    {
        
    }
}