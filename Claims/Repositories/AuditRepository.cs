using Claims.Auditing;
using Claims.Data;

namespace Claims.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditContext _auditContext;

        public AuditRepository(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        public void AddClaimAudit(ClaimAudit claimAudit)
        {
            _auditContext.AddClaimAudits(claimAudit);
        }

        public void AddCoverAudit(CoverAudit coverAudit)
        {
           _auditContext.AddCoverAudits (coverAudit);
        }
    }
}
