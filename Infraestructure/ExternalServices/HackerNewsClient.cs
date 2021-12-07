using System.Text.Json;
using Infraestructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Infraestructure.ExternalServices;
public class HackerNewsClient : IHackerNewsClient
{
    private readonly IConfiguration _config;
    private readonly HttpClient _client;
    private readonly IMemoryCache _memoryCache;
    private readonly Uri _baseUri;

    public HackerNewsClient(IConfiguration config, HttpClient client, IMemoryCache memoryCache)
    {
        _config = config;
        _client = client;
        _memoryCache = memoryCache;
        _baseUri = new Uri(_config.GetValue<string>("HackerNews:BaseUri"));
    }

    public async Task<IEnumerable<StoryResponse?>> GetBestStories(int? skip = 0, int? take = 20)
    {
        var cacheKey = $"{nameof(HackerNewsClient)}-{nameof(GetBestStories)}-{skip}-{take}".ToLower();
        var cacheAbsoluteExpiration = _config.GetValue<int>("HackerNews:Cache:AbsoluteExpiration");
        var cacheSlidingExpiration = _config.GetValue<int>("HackerNews:Cache:SlidingExpiration");

        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<StoryResponse?> response))
        {
            var bestStoriesIds = await GetBestStoriesIds(skip, take);

            var requests = new List<Task<HttpResponseMessage>>();

            foreach(var storyId in bestStoriesIds)
                requests.Add(_client.GetAsync(new Uri(_baseUri, $"/v0/item/{storyId}.json")));


            await Task.WhenAll(requests);

            var responses = requests.Select(async t => JsonSerializer.Deserialize<StoryResponse>(await t.Result.Content.ReadAsStringAsync()));

            await Task.WhenAll(responses);

            response = responses.Select(t => t.GetAwaiter().GetResult());

            var cacheExpirationOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(cacheAbsoluteExpiration),
                SlidingExpiration = TimeSpan.FromMinutes(cacheSlidingExpiration),
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(cacheKey, response, cacheExpirationOptions);
        }

        return response;
    }

    public async Task<IEnumerable<long>?> GetBestStoriesIds(int? skip = 0, int? take = 20)
    {
        var response = await _client.GetAsync(new Uri(_baseUri, "/v0/beststories.json"));
        
        if(!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to get beststories, try again later.");
        
        var responseText = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<IEnumerable<long>>(responseText).Skip(skip.Value).Take(take.Value);

        return result;
    }
}
