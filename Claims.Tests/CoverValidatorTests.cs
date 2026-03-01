using Claims.Exceptions;
using Claims.Validators;
using Xunit;

namespace Claims.Tests;

public class CoverValidatorTests
{
    private static readonly DateOnly Today = new DateOnly(2026, 3, 1);

    private static Cover MakeCover(DateOnly startDate, DateOnly endDate) => new Cover
    {
        StartDate = startDate.ToDateTime(TimeOnly.MinValue),
        EndDate = endDate.ToDateTime(TimeOnly.MinValue)
    };

    [Fact]
    public void DoesNotThrow_WhenStartDateIsToday()
    {
        var cover = MakeCover(Today, Today.AddDays(30));

        var ex = Record.Exception(() => CoverValidator.Validate(cover));

        Assert.Null(ex);
    }

    [Fact]
    public void DoesNotThrow_WhenStartDateIsFuture()
    {
        var cover = MakeCover(Today.AddDays(1), Today.AddDays(31));

        var ex = Record.Exception(() => CoverValidator.Validate(cover));

        Assert.Null(ex);
    }

    [Fact]
    public void Throws_WhenStartDateIsInPast()
    {
        var cover = MakeCover(Today.AddDays(-1), Today.AddDays(30));

        var ex = Record.Exception(() => CoverValidator.Validate(cover));

        var ve = Assert.IsType<ValidationException>(ex);
        Assert.Equal("StartDate cannot be in the past.", ve.Message);
    }

    [Fact]
    public void DoesNotThrow_WhenPeriodIsExactly365Days()
    {
        var cover = MakeCover(Today, Today.AddDays(365));

        var ex = Record.Exception(() => CoverValidator.Validate(cover));

        Assert.Null(ex);
    }

    [Fact]
    public void Throws_WhenPeriodExceeds365Days()
    {
        var cover = MakeCover(Today, Today.AddDays(366));

        var ex = Record.Exception(() => CoverValidator.Validate(cover));

        var ve = Assert.IsType<ValidationException>(ex);
        Assert.Equal("Insurance period cannot exceed 1 year.", ve.Message);
    }

    [Fact]
    public void DoesNotThrow_WhenPeriodIs364Days()
    {
        var cover = MakeCover(Today, Today.AddDays(364));

        var ex = Record.Exception(() => CoverValidator.Validate(cover));

        Assert.Null(ex);
    }
}
