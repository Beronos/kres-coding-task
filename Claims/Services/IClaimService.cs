namespace Claims.Services
{

    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetAllClaimsAsync();
        Task<Claim?> GetClaimAsync(string id);
        Task<Claim> CreateClaimAsync(Claim claim);
        Task DeleteClaimAsync(string id);
    }
}
