using Claims.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data
{
    public class AuditContext : DbContext
    {
        public AuditContext(DbContextOptions<AuditContext> options) : base(options)
        {
        }
        public DbSet<ClaimAudit> ClaimAudits { get; set; }
        public DbSet<CoverAudit> CoverAudits { get; set; }

        public void AddClaimAudits(ClaimAudit claimAudit)
        {
            ClaimAudits.Add(claimAudit);
            SaveChanges();
        }

        public void AddCoverAudits(CoverAudit coverAudit)
        {
            CoverAudits.Add(coverAudit);
            SaveChanges();
        }
    }
}
