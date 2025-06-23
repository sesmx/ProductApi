using Microsoft.Extensions.DependencyInjection;

namespace ProductApi.Infrastructure.Services;

public static class ServiceExtensions
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection s)
	{
		s.AddScoped<IDateTime, DateTimeService>();
		return s;
	}
}
