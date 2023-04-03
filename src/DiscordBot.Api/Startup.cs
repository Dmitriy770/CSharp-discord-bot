using DiscordBot.Api.Extensions;
using DiscordBot.Bll.Extensions;
using DiscordBot.Dal.Extensions;

namespace DiscordBot.Api;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services
            .AddApi(_configuration)
            .AddDalRepositories()
            .AddBll();

        return services;
    }
}