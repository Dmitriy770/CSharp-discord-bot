using DiscordBot.VoiceManager.Domain.Interfaces;
using DiscordBot.VoiceManager.Infrastructure.Extensions;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.IntegrationTests.Fixtures;

public class TestFixture
{
    public IVoiceChannelSettingsRepository VoiceChannelSettingsRepository { get; }
    public IGuildSettingsRepository GuildSettingsRepository { get; }
    public IVoiceChannelCacheRepository VoiceChannelCacheRepository { get; }

    public TestFixture()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddDalInfrastructure(config)
                    .AddDalRepositories();
            })
            .Build();

        ClearDatabase(host);
        host.MigrateUp();

        var serviceProvider = host.Services;
        VoiceChannelSettingsRepository = serviceProvider.GetRequiredService<IVoiceChannelSettingsRepository>();
        GuildSettingsRepository = serviceProvider.GetRequiredService<IGuildSettingsRepository>();
        VoiceChannelCacheRepository = serviceProvider.GetRequiredService<IVoiceChannelCacheRepository>();
    }

    private static void ClearDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(20230516);
    }
}