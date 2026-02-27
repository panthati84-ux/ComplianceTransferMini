using Microsoft.Data.SqlClient;

namespace ComplianceTransferMini.API.Infrastructure;

public sealed class DbConnectionFactory
{
    private readonly IConfiguration _config;
    public DbConnectionFactory(IConfiguration config) => _config = config;

    public SqlConnection Create()
    {
        var cs = _config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Missing connection string: Default");
        return new SqlConnection(cs);
    }
}
