using System.Text.Json;
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

    public async Task<IEnumerable<long>?> GetBestStories()
    {
        var baseUri = new Uri(_config.GetValue<string>("HackerNews:BaseUri"));
        var response = await _client.GetAsync(new Uri(baseUri, "/v0/beststories.json"));
        
        if(!response.IsSuccessStatusCode)
            return null;
        
        var responseText = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<IEnumerable<long>>(responseText);

        return result;
    }
}
