using Infraestructure.Models;

namespace Infraestructure.ExternalServices;

public interface IHackerNewsClient
{
    public Task<IEnumerable<StoryResponse?>> GetBestStories(int? skip = 0, int? take = 20);
    public Task<IEnumerable<long>?> GetBestStoriesIds(int? skip = 0, int? take = 20);
}