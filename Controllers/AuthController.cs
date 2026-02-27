using ComplianceTransferMini.API.DTOs;
using ComplianceTransferMini.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceTransferMini.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var res = await _auth.LoginAsync(req);
        return Ok(res);
    }
}
