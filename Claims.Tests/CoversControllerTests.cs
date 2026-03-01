using Claims.Controllers;
using Claims.Exceptions;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Security.Claims;
using Xunit;

namespace Claims.Tests;

public class CoversControllerTests
{
    private readonly ICoverService _coverService = Substitute.For<ICoverService>();
    private readonly ILogger<CoversController> _logger = Substitute.For<ILogger<CoversController>>();
    private readonly CoversController _coversController;

    public CoversControllerTests() => _coversController = new CoversController(_coverService, _logger);

    [Fact]
    public async Task GetAsync_ReturnsOkWithAllCovers()
    {
        var covers = new List<Cover> { new Cover { Id = "1" }, new Cover { Id = "2" } };
        _coverService.GetAllCoversAsync().Returns(Task.FromResult<IEnumerable<Cover>>(covers));

        var result = await _coversController.GetAsync();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<Cover>>(ok.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOkWithCover()
    {
        var cover = new Cover { Id = "cover-1" };
        _coverService.GetCoverAsync("cover-1").Returns(Task.FromResult<Cover?>(cover));

        var result = await _coversController.GetAsync("cover-1");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(cover, ok.Value);
    }

    [Fact]
    public async Task CreateAsync_ReturnsOk_WhenValid()
    {
        var cover = new Cover { Id = "cover-1", Type = CoverType.Yacht };
        _coverService.CreateCoverAsync(cover).Returns(Task.FromResult(cover));

        var result = await _coversController.CreateAsync(cover);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(cover, created.Value);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenValidationException()
    {
        var cover = new Cover { StartDate = DateTime.UtcNow.AddDays(-1) };
        _coverService.CreateCoverAsync(cover).ThrowsAsync(new ValidationException("StartDate cannot be in the past."));

        var result = await _coversController.CreateAsync(cover);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("StartDate cannot be in the past.", badRequest.Value);
    }

    [Fact]
    public async Task DeleteAsync_CallsService()
    {
        _coverService.DeleteCoverAsync("cover-1").Returns(Task.CompletedTask);

        var ex = await Record.ExceptionAsync(() => _coversController.DeleteAsync("cover-1"));

        Assert.Null(ex);
        await _coverService.Received(1).DeleteCoverAsync("cover-1");
    }
}
