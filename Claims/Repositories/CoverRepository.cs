using Claims.Data;

namespace Claims.Repositories
{
    public class CoverRepository : ICoverRepository
    {
        private readonly ClaimsContext _claimsContext;

        public CoverRepository(ClaimsContext claimsContext)
        {
            _claimsContext = claimsContext;
        }

        public async Task AddAsync(Cover cover)
        {
            await _claimsContext.AddCoverAsync(cover);
        }

        public async Task DeleteAsync(string id)
        {
            await _claimsContext.DeleteCoverAsync(id);
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _claimsContext.GetCoversAsync();
        }

        public async Task<Cover?> GetByIdAsync(string id)
        {
            return await _claimsContext.GetCoverAsync(id);
        }
    }
}
