namespace ProductApi.Infrastructure.Identity;

public record JwtSettings
{
	public string Issuer { get; init; } = default!;
	public string Audience { get; init; } = default!;
	public string Secret { get; init; } = default!;
	public int AccessTokenMinutes { get; init; }
	public int RefreshTokenDays { get; init; }
}

