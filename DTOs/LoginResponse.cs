namespace ComplianceTransferMini.API.DTOs;

public sealed record LoginResponse(string Token, string Email, string Role);
