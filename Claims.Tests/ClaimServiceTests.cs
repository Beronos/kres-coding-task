using Claims.Exceptions;
using Claims.Repositories;
using Claims.Services;
using NSubstitute;
using Xunit;

namespace Claims.Tests;

public class ClaimServiceTests
{
    private readonly IClaimRepository _claimRepo = Substitute.For<IClaimRepository>();
    private readonly ICoverRepository _coverRepo = Substitute.For<ICoverRepository>();
    private readonly IAuditer _auditer = Substitute.For<IAuditer>();
    private readonly ClaimService _claimService;

    public ClaimServiceTests() => _claimService = new ClaimService(_claimRepo, _coverRepo, _auditer);

    private static Cover MakeValidCover() => new Cover
    {
        Id = "cover-1",
        StartDate = new DateTime(2025, 1, 1),
        EndDate = new DateTime(2025, 12, 31),
        Type = CoverType.Yacht
    };

    [Fact]
    public async Task GetAllClaimsAsync_ReturnsAllClaims()
    {
        var claims = new List<Claim> { new Claim(), new Claim() };
        _claimRepo.GetAllAsync().Returns(Task.FromResult<IEnumerable<Claim>>(claims));

        var result = await _claimService.GetAllClaimsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetClaimAsync_ReturnsClaim_WhenFound()
    {
        var claim = new Claim { Id = "claim-1" };
        _claimRepo.GetByIdAsync("claim-1").Returns(Task.FromResult<Claim?>(claim));

        var result = await _claimService.GetClaimAsync("claim-1");

        Assert.Equal(claim, result);
    }

    [Fact]
    public async Task GetClaimAsync_ReturnsNull_WhenNotFound()
    {
        _claimRepo.GetByIdAsync("1").Returns(Task.FromResult<Claim?>(null));

        var result = await _claimService.GetClaimAsync("1");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateClaimAsync_Throws_NotFoundException_WhenCoverNotFound()
    {
        _coverRepo.GetByIdAsync("cover-1").Returns(Task.FromResult<Cover?>(null));
        var claim = new Claim { CoverId = "cover-1", DamageCost = 50_000, Created = new DateTime(2026, 3, 1) };

        var ex = await Record.ExceptionAsync(() => _claimService.CreateClaimAsync(claim));

        Assert.IsType<NotFoundException>(ex);
        await _claimRepo.DidNotReceive().AddAsync(claim);
    }

    [Fact]
    public async Task CreateClaimAsync_Throws_ValidationException_WhenDamageCostTooHigh()
    {
        var cover = MakeValidCover();
        _coverRepo.GetByIdAsync("cover-1").Returns(Task.FromResult<Cover?>(cover));
        var claim = new Claim { CoverId = "cover-1", DamageCost = 150_000, Created = new DateTime(2025, 6, 1) };

        var ex = await Record.ExceptionAsync(() => _claimService.CreateClaimAsync(claim));

        Assert.IsType<ValidationException>(ex);
        await _claimRepo.DidNotReceive().AddAsync(claim);
    }

    [Fact]
    public async Task CreateClaimAsync_AssignsId_Persists_AndAudits_WhenValid()
    {
        var cover = MakeValidCover();
        _coverRepo.GetByIdAsync("cover-1").Returns(Task.FromResult<Cover?>(cover));
        _auditer.AuditClaimAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);
        var claim = new Claim { CoverId = "cover-1", DamageCost = 50_000, Created = new DateTime(2025, 6, 1) };

        var result = await _claimService.CreateClaimAsync(claim);

        Assert.NotNull(result.Id);
        await _claimRepo.Received(1).AddAsync(claim);
        await _auditer.Received(1).AuditClaimAsync(result.Id, "POST");
    }

    [Fact]
    public async Task DeleteClaimAsync_AuditsAndDeletes()
    {
        _auditer.AuditClaimAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

        await _claimService.DeleteClaimAsync("id-1");

        await _auditer.Received(1).AuditClaimAsync("id-1", "DELETE");
        await _claimRepo.Received(1).DeleteAsync("id-1");
    }
}
