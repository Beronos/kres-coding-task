using Azure.Messaging.ServiceBus;
using Claims.Repositories;
using Claims.Services;
using System.Text.Json;

namespace Claims.Auditing
{
    public class Auditer : IAuditer
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ServiceBusSender _sender;

        public Auditer(
            IAuditRepository auditRepository,
            ServiceBusClient client,
            IConfiguration config)
        {
            _auditRepository = auditRepository;
            _sender = client.CreateSender(config["ServiceBus:QueueName"]);
        }

        public Task AuditClaimAsync(string id, string httpRequestType)
            => SendAsync("Claim", id, httpRequestType);

        public Task AuditCoverAsync(string id, string httpRequestType)
            => SendAsync("Cover", id, httpRequestType);

        private async Task SendAsync(string type, string id, string httpRequestType)
        {
            var body = JsonSerializer.Serialize(new AuditMessage(type, id, httpRequestType));
            await _sender.SendMessageAsync(new ServiceBusMessage(body));
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
