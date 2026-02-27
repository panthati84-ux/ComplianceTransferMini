namespace ComplianceTransferMini.API.Models;

public sealed class AuditEvent
{
    public Guid AuditId { get; set; }
    public Guid? RequestId { get; set; }
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = "";
    public string? Details { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }
}
