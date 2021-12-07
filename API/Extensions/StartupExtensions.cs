using AutoMapper;
using API.Profiles;
using Infraestructure.ExternalServices;
using Polly;
using Polly.Extensions.Http;

namespace API.Extensions;

public static class StartupExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        // Dependency Injection
        services.AddScoped<IHackerNewsClient, HackerNewsClient>();
        services.AddHttpClient<IHackerNewsClient, HackerNewsClient>().AddPolicyHandler(GetRetryPolicy());

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

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}