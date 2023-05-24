using FluentMigrator;

namespace DiscordBot.Dal.Migrations;

[Migration(20230518, TransactionBehavior.None)]
public class AddGuildSettingsType : Migration {
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'guilds_settings') THEN
            CREATE TYPE guild_settings as
            (
                  id                        bytea
                , create_voice_channel_id   bytea
            );
        END IF;
    END
$$;";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
DO $$
    BEGIN
        DROP TYPE IF EXISTS guild_settings;
    END
$$;";
        
        Execute.Sql(sql);
    }
}