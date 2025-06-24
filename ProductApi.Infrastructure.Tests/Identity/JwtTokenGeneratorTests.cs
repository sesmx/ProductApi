using Microsoft.Extensions.Options;
using ProductApi.Domain.Enums;
using ProductApi.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductApi.Infrastructure.Tests.Identity;

public class JwtTokenGeneratorTests
{
	private readonly JwtSettings _jwtSettings = new JwtSettings
	{
		Issuer = "TestIssuer",
		Audience = "TestAudience",
		Secret = "supersecretkey1234567890_32byteslong!!",
		AccessTokenMinutes = 60,
		RefreshTokenDays = 7
	};

	private IJwtTokenGenerator CreateGenerator(JwtSettings? settings = null)
	{
		var opts = Options.Create(settings ?? _jwtSettings);
		return new JwtTokenGenerator(opts);
	}

	private ApplicationUser CreateUser(int id = 1, string? userName = "testuser", UserRole role = UserRole.Admin)
	{
		return new ApplicationUser
		{
			Id = id,
			UserName = userName,
			Role = role,
			FullName = "Test User"
		};
	}

	[Fact]
	public void Generate_Returns_ValidTokenResponseDto()
	{
		// Arrange
		var generator = CreateGenerator();
		var user = CreateUser();

		// Act
		var result = generator.Generate(user);

		// Assert
		Assert.NotNull(result);
		Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
		Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);

		// Validate JWT structure
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(result.AccessToken);

		Assert.Equal(_jwtSettings.Issuer, jwt.Issuer);
		Assert.Equal(_jwtSettings.Audience, jwt.Audiences.First());
		Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
		Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == user.UserName);
		Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == user.Role.ToString());
	}

	[Fact]
	public void Generate_Throws_OnNullUser()
	{
		// Arrange
		var generator = CreateGenerator();

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => generator.Generate(null!));
	}

	[Fact]
	public void Generate_Uses_CorrectExpiration()
	{
		// Arrange
		var customSettings = new JwtSettings
		{
			Issuer = "Issuer",
			Audience = "Audience",
			Secret = "anothersecretkey1234567890123456",
			AccessTokenMinutes = 5,
			RefreshTokenDays = 1
		};
		var generator = CreateGenerator(customSettings);
		var user = CreateUser();

		// Act
		var result = generator.Generate(user);

		// Assert
		var expectedMin = DateTime.UtcNow.AddMinutes(4.5);
		var expectedMax = DateTime.UtcNow.AddMinutes(5.5);
		Assert.InRange(result.ExpiresAtUtc, expectedMin, expectedMax);
	}
}
