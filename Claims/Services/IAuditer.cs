namespace Claims.Services
{
    public interface IAuditer
    {
        void AuditClaim(string id, string httpRequestType);
        void AuditCover(string id, string httpRequestType);
        Task AuditClaimAsync(string id, string httpRequestType);
        Task AuditCoverAsync(string id, string httpRequestType);

    }
}
