using Claims.Controllers;
using Claims.Exceptions;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        private readonly IClaimService _claimService = Substitute.For<IClaimService>();
        private readonly ILogger<ClaimsController> _logger = Substitute.For<ILogger<ClaimsController>>();
        private ClaimsController CreateController() => new ClaimsController(_claimService, _logger);

        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});

            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("[", body.Trim());
        }

        [Fact]
        public async Task GetAsync_ReturnsAllClaims()
        {
            var claims = new List<Claim> { new Claim { Id = "1" }, new Claim { Id = "2" } };
            _claimService.GetAllClaimsAsync().Returns(Task.FromResult<IEnumerable<Claim>>(claims));

            var result = await CreateController().GetAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsClaim()
        {
            var claim = new Claim { Id = "claim-1" };
            _claimService.GetClaimAsync("claim-1").Returns(Task.FromResult<Claim?>(claim));

            var result = await CreateController().GetAsync("claim-1");

            Assert.Equal(claim, result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOk_WhenValid()
        {
            var claim = new Claim { Id = "claim-1", DamageCost = 5_000 };
            _claimService.CreateClaimAsync(claim).Returns(Task.FromResult(claim));

            var result = await CreateController().CreateAsync(claim);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
            Assert.Equal(claim, created.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFound_WhenNotFoundException()
        {
            var claim = new Claim { CoverId = "missing" };
            _claimService.CreateClaimAsync(claim).ThrowsAsync(new NotFoundException("Cover not found."));

            var result = await CreateController().CreateAsync(claim);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
            Assert.Equal("Cover not found.", notFound.Value);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_WhenValidationException()
        {
            var claim = new Claim { DamageCost = 200_000 };
            _claimService.CreateClaimAsync(claim).ThrowsAsync(new ValidationException("DamageCost cannot exceed 100,000."));

            var result = await CreateController().CreateAsync(claim);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("DamageCost cannot exceed 100,000.", badRequest.Value);
        }

        [Fact]
        public async Task DeleteAsync_CallsService()
        {
            _claimService.DeleteClaimAsync("claim-1").Returns(Task.CompletedTask);

            var ex = await Record.ExceptionAsync(() => CreateController().DeleteAsync("claim-1"));

            Assert.Null(ex);
            await _claimService.Received(1).DeleteClaimAsync("claim-1");
        }
    }
}
