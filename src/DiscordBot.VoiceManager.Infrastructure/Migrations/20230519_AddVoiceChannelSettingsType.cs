using FluentMigrator;

namespace DiscordBot.VoieManager.Infrastructure.Migrations;

[Migration(20230519, TransactionBehavior.None)]
public class AddVoiceChannelSettingsType : Migration{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'voice_channel_settings') THEN
            CREATE TYPE voice_channel_settings as
            (
                  guild_id      bytea
                , user_id       bytea
                , name          VARCHAR(15)
                , users_limit   int
                , bitrate       int
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
        DROP TYPE IF EXISTS voice_channel_settings;
    END
$$;";
        
        Execute.Sql(sql);
    }
}