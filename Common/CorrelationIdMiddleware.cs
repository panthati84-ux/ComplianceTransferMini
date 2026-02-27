namespace ComplianceTransferMini.API.Common;

public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        var cid = ctx.Request.Headers["X-Correlation-Id"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(cid))
            cid = Guid.NewGuid().ToString("N");

        ctx.Items["CorrelationId"] = cid;
        ctx.Response.Headers["X-Correlation-Id"] = cid;

        await _next(ctx);
    }
}
