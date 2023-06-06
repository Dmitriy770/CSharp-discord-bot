using FluentMigrator;

namespace DiscordBot.VoieManager.Infrastructure.Migrations;

[Migration(20230517, TransactionBehavior.None)]
public class InitSchema : Migration {
    public override void Up()
    {
        Create.Table("guilds_settings")
            .WithColumn("id").AsBinary(8).PrimaryKey("guilds_pk")
            .WithColumn("create_voice_channel_id").AsBinary(8).Nullable();

        Create.Table("voice_channels_settings")
            .WithColumn("guild_id").AsBinary(8)
            .WithColumn("user_id").AsBinary(8)
            .WithColumn("name").AsString(15).Nullable()
            .WithColumn("users_limit").AsInt32().Nullable()
            .WithColumn("bitrate").AsInt32().Nullable();

        Create.PrimaryKey("voice_channels_settings_pk")
            .OnTable("voice_channels_settings")
            .Columns(new string[]{"guild_id", "user_id"});
    }

    public override void Down()
    {
        Delete.Table("guilds_settings");
        Delete.Table("voice_channels_settings");
    }
}