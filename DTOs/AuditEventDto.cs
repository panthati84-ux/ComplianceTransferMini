namespace ComplianceTransferMini.API.DTOs;

public sealed record AuditEventDto(
  Guid AuditId,
  Guid? RequestId,
  string Action,
  string? Details,
  string? CorrelationId,
  DateTime Timestamp
);
