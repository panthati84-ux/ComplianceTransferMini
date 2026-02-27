namespace ComplianceTransferMini.API.Models;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Requester = "Requester";
    public const string Approver = "Approver";
    public const string ComplianceOfficer = "ComplianceOfficer";
    public const string Auditor = "Auditor";
}

public static class TransferStatuses
{
    public const string Draft = "Draft";
    public const string Submitted = "Submitted";
    public const string InReview = "InReview";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Sent = "Sent";
}

public static class RiskLevels
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
}
