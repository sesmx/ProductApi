using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;

namespace ProductApi.Infrastructure.Logging;

public class RequestLoggingMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
	{
		var sw = Stopwatch.StartNew();
		try
		{
			await next(ctx);
			Log.Information("{Method} {Path} => {Status} ({Elapsed:0.000} ms)",
				ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode,
				sw.Elapsed.TotalMilliseconds);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Unhandled exception for {Method} {Path}",
				ctx.Request.Method, ctx.Request.Path);
			throw;
		}
	}
}
