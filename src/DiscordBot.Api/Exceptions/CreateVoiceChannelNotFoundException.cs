namespace DiscordBot.Api.Exceptions;

public class CreateVoiceChannelNotFoundException : Exception
{
    public CreateVoiceChannelNotFoundException() : base("Create voice channel not found")
    {
        
    }
}