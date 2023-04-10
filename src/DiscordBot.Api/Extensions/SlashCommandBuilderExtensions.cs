using Discord;

namespace DiscordBot.Api.Extensions;

public static class SlashCommandBuilderExtensions
{
    public static SlashCommandBuilder AddVoiceManagerCommands(this SlashCommandBuilder slashCommandBuilder)
    {
        slashCommandBuilder
            .WithName("voice")
            .WithDescription("Commands for managing the voice channel")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("claim")
                .WithDescription("Command to capture a voice channel")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("set")
                .WithDescription("Setting the parameters of the voice channel")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("limit")
                    .WithDescription("Sets the voice channel limit")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("limit", ApplicationCommandOptionType.Integer, "Maximum number of users",
                        isRequired: true, minValue: 1, maxValue: 99))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("name")
                    .WithDescription("Sets the voice channel name")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("name", ApplicationCommandOptionType.String, "Voice channel name",
                        isRequired: true, minLength: 1, maxLength: 20)))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("reset")
                .WithDescription("Resets voice channel parameters")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("limit")
                    .WithDescription("Resets voice channel limit")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("name")
                    .WithDescription("Resets voice channel name")
                    .WithType(ApplicationCommandOptionType.SubCommand)));
        return slashCommandBuilder;
    }
}