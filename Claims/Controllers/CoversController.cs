using Claims.Exceptions;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoverService _coverService;
    private readonly ILogger<CoversController> _logger;

    public CoversController(ICoverService coverService, ILogger<CoversController> logger)
    {
        _coverService = coverService;
        _logger = logger;
    }

    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(_coverService.ComputePremium(startDate, endDate, coverType));
        
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coverService.GetAllCoversAsync();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var result = await _coverService.GetCoverAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        try
        {
            await _coverService.CreateCoverAsync(cover);
            return Ok(cover);
        }
        catch (Exception ex)
        {
            return ex switch
            {
                ValidationException => BadRequest(ex.Message),
                _ => StatusCode(500, ex.Message)
            };
        }
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _coverService.DeleteCoverAsync(id);
    }
}
