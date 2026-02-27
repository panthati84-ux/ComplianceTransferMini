using System.Security.Claims;
using System.Text.RegularExpressions;
using ComplianceTransferMini.API.Common;
using ComplianceTransferMini.API.DTOs;
using ComplianceTransferMini.API.Models;
using ComplianceTransferMini.API.Repositories;

namespace ComplianceTransferMini.API.Services;

public sealed class TransferService
{
    private readonly TransferRepository _transfers;
    private readonly AuditRepository _audit;

    public TransferService(TransferRepository transfers, AuditRepository audit)
    {
        _transfers = transfers;
        _audit = audit;
    }

    public async Task<TransferResponseDto> CreateAsync(CreateTransferRequestDto dto, ClaimsPrincipal user, string? correlationId)
    {
        var userId = GetUserId(user);

        var req = new TransferRequest
        {
            RequestId = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Recipient = dto.Recipient.Trim(),
            Purpose = dto.Purpose.Trim(),
            Status = TransferStatuses.Draft,
            RiskLevel = RiskLevels.Low,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Recipient) || string.IsNullOrWhiteSpace(req.Purpose))
            throw new ApiException("Title, Recipient, and Purpose are required.");

        await _transfers.CreateAsync(req);

        await _audit.AddAsync(new AuditEvent
        {
            AuditId = Guid.NewGuid(),
            RequestId = req.RequestId,
            ActorUserId = userId,
            Action = "TransferCreated",
            Details = $"Title='{req.Title}', Recipient='{req.Recipient}'",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });

        return Map(req);
    }

    public async Task<TransferResponseDto> SubmitAsync(Guid requestId, ClaimsPrincipal user, string? correlationId)
    {
        var userId = GetUserId(user);
        var req = await _transfers.GetByIdAsync(requestId) ?? throw new ApiException("Transfer request not found.", 404);

        if (req.CreatedByUserId != userId && !IsAdmin(user))
            throw new ApiException("You can only submit your own transfer requests.", 403);

        if (req.Status != TransferStatuses.Draft)
            throw new ApiException("Only Draft requests can be submitted.");

        // Small “policy engine”
        var risk = CalculateRisk(req.Recipient, req.Purpose);

        // Rule: Low risk -> auto approve, else -> InReview
        var newStatus = risk == RiskLevels.Low ? TransferStatuses.Approved : TransferStatuses.InReview;

        await _transfers.UpdateStatusAsync(requestId, newStatus, risk);

        await _audit.AddAsync(new AuditEvent
        {
            AuditId = Guid.NewGuid(),
            RequestId = requestId,
            ActorUserId = userId,
            Action = "TransferSubmitted",
            Details = $"Risk='{risk}', Status='{newStatus}'",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });

        var updated = await _transfers.GetByIdAsync(requestId) ?? throw new ApiException("Unexpected missing request.", 500);
        return Map(updated);
    }

    public async Task<TransferResponseDto> ApproveAsync(Guid requestId, ClaimsPrincipal user, string? comments, string? correlationId)
    {
        RequireApproverOrOfficer(user);

        var userId = GetUserId(user);
        var req = await _transfers.GetByIdAsync(requestId) ?? throw new ApiException("Transfer request not found.", 404);

        if (req.Status != TransferStatuses.InReview)
            throw new ApiException("Only InReview requests can be approved.");

        await _transfers.UpdateStatusAsync(requestId, TransferStatuses.Approved, req.RiskLevel);

        await _audit.AddAsync(new AuditEvent
        {
            AuditId = Guid.NewGuid(),
            RequestId = requestId,
            ActorUserId = userId,
            Action = "TransferApproved",
            Details = string.IsNullOrWhiteSpace(comments) ? null : $"Comments='{comments}'",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });

        var updated = await _transfers.GetByIdAsync(requestId)!;
        return Map(updated);
    }

    public async Task<TransferResponseDto> RejectAsync(Guid requestId, ClaimsPrincipal user, string? comments, string? correlationId)
    {
        RequireApproverOrOfficer(user);

        var userId = GetUserId(user);
        var req = await _transfers.GetByIdAsync(requestId) ?? throw new ApiException("Transfer request not found.", 404);

        if (req.Status != TransferStatuses.InReview)
            throw new ApiException("Only InReview requests can be rejected.");

        await _transfers.UpdateStatusAsync(requestId, TransferStatuses.Rejected, req.RiskLevel);

        await _audit.AddAsync(new AuditEvent
        {
            AuditId = Guid.NewGuid(),
            RequestId = requestId,
            ActorUserId = userId,
            Action = "TransferRejected",
            Details = string.IsNullOrWhiteSpace(comments) ? null : $"Comments='{comments}'",
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        });

        var updated = await _transfers.GetByIdAsync(requestId)!;
        return Map(updated);
    }

    public async Task<IEnumerable<TransferResponseDto>> ListAsync(string? status)
    {
        var list = await _transfers.ListAsync(status);
        return list.Select(Map);
    }

    private static TransferResponseDto Map(TransferRequest r) =>
      new(r.RequestId, r.Title, r.Recipient, r.Purpose, r.Status, r.RiskLevel, r.CreatedAt, r.UpdatedAt);

    private static Guid GetUserId(ClaimsPrincipal user)
    {
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(idStr, out var id)) throw new ApiException("Invalid user identity.", 401);
        return id;
    }

    private static bool IsAdmin(ClaimsPrincipal user) =>
      user.IsInRole(Roles.Admin);

    private static void RequireApproverOrOfficer(ClaimsPrincipal user)
    {
        if (!(user.IsInRole(Roles.Approver) || user.IsInRole(Roles.ComplianceOfficer) || user.IsInRole(Roles.Admin)))
            throw new ApiException("Approver/ComplianceOfficer role required.", 403);
    }

    private static string CalculateRisk(string recipient, string purpose)
    {
        // internal domains -> lower risk
        var internalRecipient = recipient.EndsWith("@corp.com", StringComparison.OrdinalIgnoreCase);

        // basic PII scan demo (email/phone-like/ssn-like)
        var pii = Regex.IsMatch(purpose, @"\b\d{3}-\d{2}-\d{4}\b") // SSN-like
                  || Regex.IsMatch(purpose, @"\b\d{10}\b")        // phone-like
                  || Regex.IsMatch(purpose, @"\b\S+@\S+\.\S+\b"); // email

        if (pii) return RiskLevels.High;
        if (!internalRecipient) return RiskLevels.Medium;
        return RiskLevels.Low;
    }
}
