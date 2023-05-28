using DiscordBot.Bll.Entities;
using DiscordBot.Dal.Settings;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.NameTranslation;

namespace DiscordBot.Dal.Infrastructure;

public class Postgres
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

    public static void MapCompositeTypes()
    {
        var mapper = new NpgsqlDataSourceBuilder();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        mapper.MapComposite<GuildSettingsEntity>("guilds_settings", Translator);
        mapper.MapComposite<VoiceChannelSettingsEntity>("voice_channel_settings", Translator);

    }

    public static void AddMigrations(IServiceCollection services)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(s =>
                {
                    var dalSettings = s.GetRequiredService<IOptions<DalOptions>>().Value;
                    var connectionStringBuilder = new NpgsqlConnectionStringBuilder
                    {
                        Host = dalSettings.Host,
                        Port = dalSettings.Port,
                        Username = dalSettings.User,
                        Password = dalSettings.Password,
                        Database = dalSettings.Database,
                        Pooling = dalSettings.Pooling
                    };

                    return connectionStringBuilder.ConnectionString;
                })
                .ScanIn(typeof(Postgres).Assembly).For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
}