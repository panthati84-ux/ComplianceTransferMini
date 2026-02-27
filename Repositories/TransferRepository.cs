using ComplianceTransferMini.API.Infrastructure;
using ComplianceTransferMini.API.Models;
using Dapper;

namespace ComplianceTransferMini.API.Repositories;

public sealed class TransferRepository
{
    private readonly DbConnectionFactory _db;
    public TransferRepository(DbConnectionFactory db) => _db = db;

    public async Task CreateAsync(TransferRequest req)
    {
        const string sql = @"
      INSERT INTO dbo.TransferRequests
      (RequestId, Title, Recipient, Purpose, Status, RiskLevel, CreatedByUserId, CreatedAt, UpdatedAt)
      VALUES
      (@RequestId, @Title, @Recipient, @Purpose, @Status, @RiskLevel, @CreatedByUserId, @CreatedAt, @UpdatedAt);
    ";
        using var conn = _db.Create();
        await conn.ExecuteAsync(sql, req);
    }

    public async Task<TransferRequest?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM dbo.TransferRequests WHERE RequestId = @Id;";
        using var conn = _db.Create();
        return await conn.QueryFirstOrDefaultAsync<TransferRequest>(sql, new { Id = id });
    }

    public async Task<IEnumerable<TransferRequest>> ListAsync(string? status = null)
    {
        var sql = "SELECT * FROM dbo.TransferRequests ";
        if (!string.IsNullOrWhiteSpace(status))
            sql += "WHERE Status = @Status ";
        sql += "ORDER BY CreatedAt DESC;";

        using var conn = _db.Create();
        return await conn.QueryAsync<TransferRequest>(sql, new { Status = status });
    }

    public async Task UpdateStatusAsync(Guid id, string status, string riskLevel)
    {
        const string sql = @"
      UPDATE dbo.TransferRequests
      SET Status = @Status, RiskLevel = @RiskLevel, UpdatedAt = SYSDATETIME()
      WHERE RequestId = @Id;
    ";
        using var conn = _db.Create();
        await conn.ExecuteAsync(sql, new { Id = id, Status = status, RiskLevel = riskLevel });
    }
}
