using DiscordBot.Api.Services;
using DiscordBot.Dal.Extensions;

namespace DiscordBot.Api;

class Program
{
    public static Task Main(string[] args)
    {
        var host = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(
                (hostBuilderContext, serviceCollection) =>
                    new Startup(hostBuilderContext.Configuration).ConfigureServices(serviceCollection)
            ).Build();

        host.Services.GetService<StartupService>()?.RunAsync();
        host.Services.GetService<LogService>();
        host.Services.GetService<AdminsCommandHandlerService>()?.RunAsync();
        host.Services.GetService<VoiceManagerCommandHandlerService>()?.RunAsync();
        host.Services.GetService<VoiceChannelService>()?.RunAsync();

        host.MigrateUp();

        return host.RunAsync();
    }
}