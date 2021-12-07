using API.Models;
using AutoMapper;
using Infraestructure.ExternalServices;
using Infraestructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/")]
public class HackerNewsController : ControllerBase
{
    private readonly ILogger<HackerNewsController> _logger;
    private readonly IHackerNewsClient _hackerNewsClient;
    private readonly IMapper _mapper;

    public HackerNewsController(ILogger<HackerNewsController> logger, IHackerNewsClient hackerNewsClient, IMapper mapper)
    {
        _hackerNewsClient = hackerNewsClient;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("best20", Name = "GetBestStories")]
    public async Task<IEnumerable<BestStoryViewModel>> Get()
    {
        var response = await _hackerNewsClient.GetBestStories();
        
        var result = _mapper.Map<IEnumerable<BestStoryViewModel>>(response);

        return result;
    }
}
