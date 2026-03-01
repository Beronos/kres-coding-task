using Claims.Exceptions;
using Claims.Validators;
using Xunit;

namespace Claims.Tests;

public class ClaimValidatorTests
{
    private static Cover MakeCover() => new Cover
    {
        Id = "cover-1",
        StartDate = new DateTime(2025, 1, 1),
        EndDate = new DateTime(2025, 12, 31),
        Type = CoverType.Yacht
    };

    [Fact]
    public void DoesNotThrow_WhenValid()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 50_000, Created = new DateTime(2025, 6, 1) };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        Assert.Null(ex);
    }

    [Fact]
    public void DoesNotThrow_WhenDamageCostExactlyAtLimit()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 100_000, Created = new DateTime(2025, 6, 1) };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        Assert.Null(ex);
    }

    [Fact]
    public void Throws_WhenDamageCostExceedsLimit()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 100_001, Created = new DateTime(2025, 6, 1) };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        var ve = Assert.IsType<ValidationException>(ex);
        Assert.Equal("DamageCost cannot exceed 100,000.", ve.Message);
    }

    [Fact]
    public void DoesNotThrow_WhenCreatedOnStartDate()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 50_000, Created = cover.StartDate };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        Assert.Null(ex);
    }

    [Fact]
    public void DoesNotThrow_WhenCreatedOnEndDate()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 50_000, Created = cover.EndDate };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        Assert.Null(ex);
    }

    [Fact]
    public void Throws_WhenCreatedBeforeCoverStart()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 50_000, Created = cover.StartDate.AddDays(-1) };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        var ve = Assert.IsType<ValidationException>(ex);
        Assert.Equal("Claim Created date must be within the cover period.", ve.Message);
    }

    [Fact]
    public void Throws_WhenCreatedAfterCoverEnd()
    {
        var cover = MakeCover();
        var claim = new Claim { DamageCost = 50_000, Created = cover.EndDate.AddDays(1) };

        var ex = Record.Exception(() => ClaimValidator.Validate(claim, cover));

        var ve = Assert.IsType<ValidationException>(ex);
        Assert.Equal("Claim Created date must be within the cover period.", ve.Message);
    }
}
