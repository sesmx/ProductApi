namespace ProductApi.Infrastructure.Services;

public interface IDateTime
{
	DateTime UtcNow { get; }
}

public class DateTimeService : IDateTime
{
	public DateTime UtcNow => DateTime.UtcNow;
}
