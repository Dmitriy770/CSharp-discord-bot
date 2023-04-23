namespace DiscordBot.Bll.Models;

public record VoicePropertiesModel
{
    public string? Name { get; init; }
    public int? UserLimit { get; init; }
}