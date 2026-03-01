using Claims.Exceptions;
using Claims.Repositories;
using Claims.Validators;

namespace Claims.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly ICoverRepository _coverRepository;
        private readonly IAuditer _auditer;

        public ClaimService(IClaimRepository claimRepository, ICoverRepository coverRepository, IAuditer auditer)
        {
            this._claimRepository = claimRepository;
            this._coverRepository = coverRepository;
            this._auditer = auditer;
        }

        public async Task<Claim> CreateClaimAsync(Claim claim)
        {
            var cover = await _coverRepository.GetByIdAsync(claim.CoverId);
            if (cover is null)
                throw new NotFoundException("Cover not found.");
            ClaimValidator.Validate(claim, cover);

            claim.Id = Guid.NewGuid().ToString();
            await _claimRepository.AddAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return claim;
        }

        public async Task DeleteClaimAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await _claimRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
        {
            return await _claimRepository.GetAllAsync();
        }

        public async Task<Claim?> GetClaimAsync(string id)
        {
            return await _claimRepository.GetByIdAsync(id);
        }
    }
}
