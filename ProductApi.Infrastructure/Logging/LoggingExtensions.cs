using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ProductApi.Infrastructure.Logging;

public static class LoggingExtensions
{
	public static IServiceCollection AddStructuredLogging(this IServiceCollection services, IConfiguration cfg)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(cfg)
			.Enrich.FromLogContext()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.CreateLogger();

		services.AddLogging(loggingBuilder =>
		{
			loggingBuilder.ClearProviders();
			loggingBuilder.AddSerilog(Log.Logger, dispose: true);
		});

		return services;
	}
}
