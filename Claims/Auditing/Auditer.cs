using Claims.Repositories;
using Claims.Services;

namespace Claims.Auditing
{
    public class Auditer : IAuditer
    {
        private readonly IAuditRepository _auditRepository;

        public Auditer(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public void AuditClaim(string id, string httpRequestType)
        {
            _auditRepository.AddClaimAudit(new ClaimAudit
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            });
        }

        public void AuditCover(string id, string httpRequestType)
        {
            _auditRepository.AddCoverAudit(new CoverAudit
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            });
        }
    }
}
