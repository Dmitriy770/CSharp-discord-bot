using DiscordBot.Api.Extensions;
using DiscordBot.Api.Services;
using DiscordBot.Bll.Extensions;
using DiscordBot.Dal.Extensions;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services
        .AddApi(context.Configuration)
        .AddBll()
        .AddDalInfrastructure(context.Configuration)
        .AddDalRepositories();
});

var host = builder.Build();

host.Services.GetService<StartupService>()?.RunAsync();
host.Services.GetService<LogService>()?.RunAsync();
host.Services.GetService<AdminsCommandHandlerService>()?.RunAsync();
host.Services.GetService<VoiceManagerCommandHandlerService>()?.RunAsync();
host.Services.GetService<VoiceChannelService>()?.RunAsync();

host.MigrateUp();

await host.RunAsync();