using ComplianceTransferMini.API.DTOs;
using ComplianceTransferMini.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceTransferMini.API.Controllers;

[ApiController]
[Route("api/transfers/{id:guid}/audit")]
[Authorize]
public sealed class AuditController : ControllerBase
{
    private readonly AuditRepository _audit;
    public AuditController(AuditRepository audit) => _audit = audit;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditEventDto>>> Get(Guid id)
    {
        var events = await _audit.GetByRequestIdAsync(id);
        var res = events.Select(e => new AuditEventDto(
          e.AuditId, e.RequestId, e.Action, e.Details, e.CorrelationId, e.Timestamp
        ));
        return Ok(res);
    }
}
