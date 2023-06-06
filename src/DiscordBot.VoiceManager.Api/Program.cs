using DiscordBot.VoiceManager.Api.Extensions;
using DiscordBot.VoiceManager.Api.Services;
using DiscordBot.VoiceManager.Domain.Extensions;
using DiscordBot.VoiceManager.Infrastructure.Extensions;

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