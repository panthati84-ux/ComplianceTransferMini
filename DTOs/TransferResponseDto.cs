namespace ComplianceTransferMini.API.DTOs;

public sealed record TransferResponseDto(
  Guid RequestId,
  string Title,
  string Recipient,
  string Purpose,
  string Status,
  string RiskLevel,
  DateTime CreatedAt,
  DateTime UpdatedAt
);
