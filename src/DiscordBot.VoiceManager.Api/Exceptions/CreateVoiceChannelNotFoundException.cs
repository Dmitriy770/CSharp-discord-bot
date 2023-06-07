namespace DiscordBot.VoiceManager.Api.Exceptions;

public class CreateVoiceChannelNotFoundException : Exception
{
    public CreateVoiceChannelNotFoundException() : base("Create voice channel not found")
    {
        
    }
}