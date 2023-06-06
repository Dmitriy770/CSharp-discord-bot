using Discord;

namespace DiscordBot.VoiceManager.Api.Extensions;

public static class SlashCommandBuilderExtensions
{
    public static SlashCommandBuilder AddVoiceSettingsCommand(this SlashCommandBuilder slashCommandBuilder)
    {
        slashCommandBuilder
            .WithName("voice")
            .WithDescription("Commands for managing the voice channel")
            .WithDMPermission(false)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("claim")
                .WithDescription("Command to capture a voice channel")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("info")
                .WithDescription("Shows information about the voice channel")
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
                        isRequired: true, minLength: 1, maxLength: 20))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("bitrate")
                    .WithDescription("Sets the voice channel bitrate")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("bitrate", ApplicationCommandOptionType.Number, "Voice channel bitrate in kbps",
                        isRequired: true, minValue: 8, maxValue: 384)))
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
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("bitrate")
                    .WithDescription("Resets voice channel bitrate")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("all")
                    .WithDescription("Resets all voice channel settings")
                    .WithType(ApplicationCommandOptionType.SubCommand)));
        return slashCommandBuilder;
    }

    public static SlashCommandBuilder AddAdminCommand(this SlashCommandBuilder slashCommandBuilder)
    {
        slashCommandBuilder
            .WithName("voice-admin")
            .WithDescription("Commands for setting voice manager")
            .WithDMPermission(false)
            .WithDefaultMemberPermissions(GuildPermission.Administrator)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("create")
                .WithDescription("Сommand to create")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("create-voice-channel")
                    .WithDescription("Create channel to create other channels")
                    .WithType(ApplicationCommandOptionType.SubCommand)))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("set")
                .WithDescription("Command to set")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("create-voice-channel")
                    .WithDescription("Set channel to create other channels")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("create-voice-channel", ApplicationCommandOptionType.Channel,
                        "сhannel to create other channels", isRequired: true)
                ));

        return slashCommandBuilder;
    }
}