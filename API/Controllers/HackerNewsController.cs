using Infraestructure.ExternalServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/")]
public class HackerNewsController : ControllerBase
{
    private readonly ILogger<HackerNewsController> _logger;
    private readonly IHackerNewsClient _hackerNewsClient;

    public HackerNewsController(ILogger<HackerNewsController> logger, IHackerNewsClient hackerNewsClient)
    {
        _hackerNewsClient = hackerNewsClient;
        _logger = logger;
    }

    [HttpGet("best20", Name = "GetBestStories")]
    public async Task<IEnumerable<long>?> Get()
    {
        return await _hackerNewsClient.GetBestStories();
    }
}
