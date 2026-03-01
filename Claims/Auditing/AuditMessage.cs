namespace Claims.Auditing;

public record AuditMessage(string Type, string EntityId, string HttpRequestType);
