using FluentMigrator;

namespace DiscordBot.Dal.Migrations;

[Migration(20230517, TransactionBehavior.None)]
public class InitSchema : Migration {
    public override void Up()
    {
        Create.Table("guilds_settings")
            .WithColumn("id").AsInt64().PrimaryKey("guilds_pk")
            .WithColumn("create_voice_channel_id").AsInt64().Nullable();

        Create.Table("channels_settings")
            .WithColumn("guild_id").AsInt64()
            .WithColumn("user_id").AsInt64()
            .WithColumn("name").AsFixedLengthString(15).Nullable()
            .WithColumn("user_limit").AsInt32().Nullable()
            .WithColumn("bitrate").AsInt32().Nullable();

        Create.PrimaryKey("channels_settings_pk")
            .OnTable("channels_settings")
            .Columns(new string[]{"guild_id", "user_id"});
    }

    public override void Down()
    {
        Delete.Table("guilds");
        Delete.Table("channels_settings");
    }
}