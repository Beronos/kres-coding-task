using Claims.Auditing;

namespace Claims.Repositories
{
    public interface IAuditRepository
    {
        void AddClaimAudit(ClaimAudit claimAudit);
        void AddCoverAudit(CoverAudit coverAudit);
    }
}
