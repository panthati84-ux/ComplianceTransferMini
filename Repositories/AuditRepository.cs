using ComplianceTransferMini.API.Infrastructure;
using ComplianceTransferMini.API.Models;
using Dapper;

namespace ComplianceTransferMini.API.Repositories;

public sealed class AuditRepository
{
    private readonly DbConnectionFactory _db;
    public AuditRepository(DbConnectionFactory db) => _db = db;

    public async Task AddAsync(AuditEvent ev)
    {
        const string sql = @"
      INSERT INTO dbo.AuditEvents
      (AuditId, RequestId, ActorUserId, Action, Details, CorrelationId, Timestamp)
      VALUES
      (@AuditId, @RequestId, @ActorUserId, @Action, @Details, @CorrelationId, @Timestamp);
    ";
        using var conn = _db.Create();
        await conn.ExecuteAsync(sql, ev);
    }

    public async Task<IEnumerable<AuditEvent>> GetByRequestIdAsync(Guid requestId)
    {
        const string sql = @"
      SELECT * FROM dbo.AuditEvents
      WHERE RequestId = @RequestId
      ORDER BY Timestamp ASC;
    ";
        using var conn = _db.Create();
        return await conn.QueryAsync<AuditEvent>(sql, new { RequestId = requestId });
    }
}
