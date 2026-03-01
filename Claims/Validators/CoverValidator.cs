using Claims.Exceptions;

namespace Claims.Validators;

public static class CoverValidator
{
    public static void Validate(Cover cover)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (DateOnly.FromDateTime(cover.StartDate) < today)
        {
            throw new ValidationException("StartDate cannot be in the past.");
        }
        if ((cover.EndDate - cover.StartDate).TotalDays > 365)
        {
            throw new ValidationException("Insurance period cannot exceed 1 year.");
        }
    }
}
