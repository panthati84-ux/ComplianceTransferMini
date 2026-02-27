using ComplianceTransferMini.API.DTOs;
using ComplianceTransferMini.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceTransferMini.API.Controllers;

[ApiController]
[Route("api/transfers")]
[Authorize]
public sealed class TransfersController : ControllerBase
{
    private readonly TransferService _svc;
    public TransfersController(TransferService svc) => _svc = svc;

    [HttpPost]
    public async Task<ActionResult<TransferResponseDto>> Create([FromBody] CreateTransferRequestDto dto)
    {
        var cid = HttpContext.Items["CorrelationId"] as string;
        var res = await _svc.CreateAsync(dto, User, cid);
        return Ok(res);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<TransferResponseDto>> Submit(Guid id)
    {
        var cid = HttpContext.Items["CorrelationId"] as string;
        var res = await _svc.SubmitAsync(id, User, cid);
        return Ok(res);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransferResponseDto>>> List([FromQuery] string? status = null)
    {
        var res = await _svc.ListAsync(status);
        return Ok(res);
    }
}
