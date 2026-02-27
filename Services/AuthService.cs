using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComplianceTransferMini.API.Common;
using ComplianceTransferMini.API.DTOs;
using ComplianceTransferMini.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ComplianceTransferMini.API.Services;

public sealed class AuthService
{
    private readonly UserRepository _users;
    private readonly IConfiguration _config;

    public AuthService(UserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        var user = await _users.ValidateUserAsync(req.Email, req.Password);
        if (user is null) throw new ApiException("Invalid credentials", 401);

        var jwt = _config.GetSection("Jwt");
        var key = jwt["Key"]!;
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;

        var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Value.userId.ToString()),
      new(ClaimTypes.NameIdentifier, user.Value.userId.ToString()),
      new(ClaimTypes.Email, user.Value.email),
      new(ClaimTypes.Role, user.Value.role)
    };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
          issuer: issuer,
          audience: audience,
          claims: claims,
          expires: DateTime.UtcNow.AddHours(4),
          signingCredentials: creds
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return new LoginResponse(tokenStr, user.Value.email, user.Value.role);
    }
}
