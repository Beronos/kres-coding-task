using Claims.Exceptions;

namespace Claims.Validators;

public static class ClaimValidator
{
    public static void Validate(Claim claim, Cover cover)
    {
        if (claim.DamageCost > 100_000)
            throw new ValidationException("DamageCost cannot exceed 100,000.");
        if (claim.Created.Date < cover.StartDate.Date || claim.Created.Date > cover.EndDate.Date)
            throw new ValidationException("Claim Created date must be within the cover period.");
    }
}
