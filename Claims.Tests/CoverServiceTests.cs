using Claims.Exceptions;
using Claims.Repositories;
using Claims.Services;
using NSubstitute;
using Xunit;

namespace Claims.Tests;

public class CoverServiceTests
{
    private readonly ICoverRepository _coverRepo = Substitute.For<ICoverRepository>();
    private readonly IAuditer _auditer = Substitute.For<IAuditer>();
    private readonly CoverService _coverService;

    public CoverServiceTests() => _coverService = new CoverService(_coverRepo, _auditer);

    [Fact]
    public async Task GetAllCoversAsync_ReturnsAllCovers()
    {
        var covers = new List<Cover> { new Cover(), new Cover() };
        _coverRepo.GetAllAsync().Returns(Task.FromResult<IEnumerable<Cover>>(covers));

        var result = await _coverService.GetAllCoversAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateCoverAsync_Throws_WhenStartDateInPast()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var cover = new Cover { StartDate = yesterday, EndDate = yesterday.AddDays(30) };

        var ex = await Record.ExceptionAsync(() => _coverService.CreateCoverAsync(cover));

        Assert.IsType<ValidationException>(ex);
        await _coverRepo.DidNotReceive().AddAsync(cover);
    }

    [Fact]
    public async Task CreateCoverAsync_Throws_WhenPeriodExceeds365Days()
    {
        var today = DateTime.UtcNow.Date;
        var cover = new Cover { StartDate = today, EndDate = today.AddDays(366) };

        var ex = await Record.ExceptionAsync(() => _coverService.CreateCoverAsync(cover));

        Assert.IsType<ValidationException>(ex);
        await _coverRepo.DidNotReceive().AddAsync(cover);
    }

    [Fact]
    public async Task CreateCoverAsync_AssignsId_ComputesPremium_Persists_AndAudits()
    {
        var today = DateTime.UtcNow.Date;
        var cover = new Cover { StartDate = today, EndDate = today.AddDays(30), Type = CoverType.Yacht };
        _auditer.AuditCoverAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

        var result = await _coverService.CreateCoverAsync(cover);

        Assert.NotNull(result.Id);
        Assert.True(result.Premium > 0);
        await _coverRepo.Received(1).AddAsync(Arg.Any<Cover>());
        await _auditer.Received(1).AuditCoverAsync(result.Id, "POST");
    }

    [Fact]
    public async Task DeleteCoverAsync_AuditsAndDeletes()
    {
        _auditer.AuditCoverAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

        await _coverService.DeleteCoverAsync("id-1");

        await _auditer.Received(1).AuditCoverAsync("id-1", "DELETE");
        await _coverRepo.Received(1).DeleteAsync("id-1");
    }
}
