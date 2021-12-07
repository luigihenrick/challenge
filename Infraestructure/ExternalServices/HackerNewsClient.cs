using System.Text.Json;
using Infraestructure.Models;
using Microsoft.Extensions.Configuration;

namespace Infraestructure.ExternalServices;
public class HackerNewsClient : IHackerNewsClient
{
    private readonly IConfiguration _config;
    private readonly HttpClient _client;

    public HackerNewsClient(IConfiguration config, HttpClient client)
    {
        this._config = config;
        this._client = client;
    }

    public async Task<IEnumerable<StoryResponse>> GetBestStories(int? skip = 0, int? take = 20)
    {
        var baseUri = new Uri(_config.GetValue<string>("HackerNews:BaseUri"));
        var bestStoriesIds = await GetBestStoriesIds(skip, take);

        var response = new List<StoryResponse>();

        foreach(var storyId in bestStoriesIds)
        {
            var storyRequest = await _client.GetAsync(new Uri(baseUri, $"/v0/item/{storyId}.json"));

            var storyResponse = await storyRequest.Content.ReadAsStringAsync();

            response.Add(JsonSerializer.Deserialize<StoryResponse>(storyResponse));
        }

        return response;
    }

    public async Task<IEnumerable<long>?> GetBestStoriesIds(int? skip = 0, int? take = 20)
    {
        var baseUri = new Uri(_config.GetValue<string>("HackerNews:BaseUri"));
        var response = await _client.GetAsync(new Uri(baseUri, "/v0/beststories.json"));
        
        if(!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to get beststories, try again later.");
        
        var responseText = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<IEnumerable<long>>(responseText).Skip(skip.Value).Take(take.Value);

        return result;
    }
}
