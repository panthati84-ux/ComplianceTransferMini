using ComplianceTransferMini.API.DTOs;
using ComplianceTransferMini.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceTransferMini.API.Controllers;

[ApiController]
[Route("api/transfers/{id:guid}")]
[Authorize]
public sealed class ApprovalsController : ControllerBase
{
    private readonly TransferService _svc;
    public ApprovalsController(TransferService svc) => _svc = svc;

    [HttpPost("approve")]
    public async Task<ActionResult<TransferResponseDto>> Approve(Guid id, [FromBody] DecisionDto dto)
    {
        var cid = HttpContext.Items["CorrelationId"] as string;
        var res = await _svc.ApproveAsync(id, User, dto.Comments, cid);
        return Ok(res);
    }

    [HttpPost("reject")]
    public async Task<ActionResult<TransferResponseDto>> Reject(Guid id, [FromBody] DecisionDto dto)
    {
        var cid = HttpContext.Items["CorrelationId"] as string;
        var res = await _svc.RejectAsync(id, User, dto.Comments, cid);
        return Ok(res);
    }
}
