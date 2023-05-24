using DiscordBot.Bll.Interfaces;
using DiscordBot.Dal.Extensions;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.IntegrationTests.Fixtures;

public class TestFixture
{
    public IVoiceChannelSettingsRepository VoiceChannelSettingsRepository{ get; }

    public TestFixture()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
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
    }

    private static void ClearDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(20230516);
    }

}