using AutoMapper;
using API.Profiles;
using Infraestructure.ExternalServices;

namespace API.Extensions;

public static class StartupExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        // Dependency Injection
        services.AddScoped<IHackerNewsClient, HackerNewsClient>();
        services.AddHttpClient<IHackerNewsClient, HackerNewsClient>();

        // Cache
        services.AddMemoryCache();

        // Auto Mapper Configurations
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new StoryProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }
}