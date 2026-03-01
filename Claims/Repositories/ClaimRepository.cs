using Claims.Data;

namespace Claims.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext _claimsContext;

        public ClaimRepository(ClaimsContext claimsContext) 
        {
            _claimsContext = claimsContext;
        }
        public async Task AddAsync(Claim claim)
        {
             await _claimsContext.AddItemAsync(claim);
        }

        public async Task DeleteAsync(string id)
        {
            await _claimsContext.DeleteItemAsync(id);
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _claimsContext.GetClaimsAsync();
        }

        public async Task<Claim?> GetByIdAsync(string id)
        {
            return await _claimsContext.GetClaimAsync(id);
        }
    }
}
