using AutoMapper;
using API.Profiles;
using Infraestructure.ExternalServices;
using Polly;
using Polly.Extensions.Http;

namespace API.Extensions;

public static class StartupExtensions
{
    public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var pollyMaxRetryCount = configuration.GetValue<int?>("HackerNews:Polly:MaxRetryCount");
        var pollySleepDurationInSeconds = configuration.GetValue<int?>("HackerNews:Polly:SleepDurationInSeconds");
        
        // Dependency Injection
        services.AddScoped<IHackerNewsClient, HackerNewsClient>();
        services.AddHttpClient<IHackerNewsClient, HackerNewsClient>()
                .AddPolicyHandler(GetRetryPolicy(pollyMaxRetryCount, pollySleepDurationInSeconds));

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

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int? maxRetryCount, int? sleepDurationInSeconds)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(maxRetryCount ?? 5, retryAttempt => TimeSpan.FromSeconds(sleepDurationInSeconds ?? Math.Pow(2, retryAttempt)));
    }
}