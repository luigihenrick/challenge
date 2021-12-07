namespace Infraestructure.ExternalServices;

public interface IHackerNewsClient
{
    public Task<IEnumerable<long>?> GetBestStories();
}