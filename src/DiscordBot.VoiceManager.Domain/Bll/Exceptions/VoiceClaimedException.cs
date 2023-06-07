namespace DiscordBot.VoiceManager.Domain.Bll.Exceptions;

public class VoiceClaimedException : Exception
{
    public VoiceClaimedException() : base("The voice channel is already yours")
    {
        
    }
}