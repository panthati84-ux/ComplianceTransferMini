using ComplianceTransferMini.API.Infrastructure;
using Dapper;

namespace ComplianceTransferMini.API.Repositories;

public sealed class UserRepository
{
    private readonly DbConnectionFactory _db;
    public UserRepository(DbConnectionFactory db) => _db = db;

    public async Task<(Guid userId, string email, string role)?> ValidateUserAsync(string email, string password)
    {
        const string sql = @"
      SELECT TOP 1 UserId, Email, Role
      FROM dbo.Users
      WHERE Email = @Email AND PasswordHash = @Password;
    ";

        using var conn = _db.Create();
        var row = await conn.QueryFirstOrDefaultAsync(sql, new { Email = email, Password = password });
        if (row is null) return null;

        return ((Guid)row.UserId, (string)row.Email, (string)row.Role);
    }
}
