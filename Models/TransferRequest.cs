namespace ComplianceTransferMini.API.Models;

public sealed class TransferRequest
{
    public Guid RequestId { get; set; }
    public string Title { get; set; } = "";
    public string Recipient { get; set; } = "";
    public string Purpose { get; set; } = "";
    public string Status { get; set; } = TransferStatuses.Draft;
    public string RiskLevel { get; set; } = RiskLevels.Low;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
