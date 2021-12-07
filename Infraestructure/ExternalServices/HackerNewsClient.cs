using System.Text.Json;
using Infraestructure.Models;
using Microsoft.Extensions.Configuration;

namespace Infraestructure.ExternalServices;
public class HackerNewsClient : IHackerNewsClient
{
    private readonly IConfiguration _config;
    private readonly HttpClient _client;
    private readonly Uri _baseUri;

    public HackerNewsClient(IConfiguration config, HttpClient client)
    {
        _config = config;
        _client = client;
        _baseUri = new Uri(_config.GetValue<string>("HackerNews:BaseUri"));
    }

    public async Task<IEnumerable<StoryResponse?>> GetBestStories(int? skip = 0, int? take = 20)
    {
        var bestStoriesIds = await GetBestStoriesIds(skip, take);

        var requests = new List<Task<HttpResponseMessage>>();

        foreach(var storyId in bestStoriesIds)
            requests.Add(_client.GetAsync(new Uri(_baseUri, $"/v0/item/{storyId}.json")));


        await Task.WhenAll(requests);

        var responses = requests.Select(async t => JsonSerializer.Deserialize<StoryResponse>(await t.Result.Content.ReadAsStringAsync()));

        await Task.WhenAll(responses);

        return responses.Select(t => t.GetAwaiter().GetResult());
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
