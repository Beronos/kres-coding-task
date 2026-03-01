using Claims.Exceptions;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {

        private readonly IClaimService _claimService;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(IClaimService claimService, ILogger<ClaimsController> logger)
        {
            _logger = logger;
            _claimService = claimService;
        }

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _claimService.GetAllClaimsAsync();
        }

        [HttpPost]
        [ActionName("GetById")]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            try
            {
                await _claimService.CreateClaimAsync(claim);
                return CreatedAtAction("GetById", new { id = claim.Id }, claim);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    NotFoundException => NotFound(ex.Message),
                    ValidationException => BadRequest(ex.Message),
                    _ => StatusCode(500, ex.Message)
                };
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _claimService.DeleteClaimAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            return await _claimService.GetClaimAsync(id);
        }
    }
}
