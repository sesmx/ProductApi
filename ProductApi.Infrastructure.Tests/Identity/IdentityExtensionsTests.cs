using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Infrastructure.Identity;

namespace ProductApi.Infrastructure.Tests.Identity;

public class IdentityExtensionsTests
{
	[Fact]
	public void AddIdentityInfrastructure_RegistersJwtTokenGenerator()
	{
		// Arrange
		var services = new ServiceCollection();
		var config = BuildConfiguration();

		// Act
		services.AddIdentityInfrastructure(config);

		// Assert
		var provider = services.BuildServiceProvider();
		var jwtTokenGenerator = provider.GetService<IJwtTokenGenerator>();
		Assert.NotNull(jwtTokenGenerator);
	}

	[Fact]
	public void AddIdentityInfrastructure_RegistersCurrentUserService()
	{
		// Arrange
		var services = new ServiceCollection();
		var config = BuildConfiguration();

		// Act
		services.AddIdentityInfrastructure(config);

		// Assert
		var provider = services.BuildServiceProvider();
		var currentUser = provider.GetService<ICurrentUser>();
		Assert.NotNull(currentUser);
	}

	[Fact]
	public void AddIdentityInfrastructure_RegistersHttpContextAccessor()
	{
		// Arrange
		var services = new ServiceCollection();
		var config = BuildConfiguration();

		// Act
		services.AddIdentityInfrastructure(config);

		// Assert
		var provider = services.BuildServiceProvider();
		var accessor = provider.GetService<IHttpContextAccessor>();
		Assert.NotNull(accessor);
	}

	[Fact]
	public void AddIdentityInfrastructure_RegistersAuthentication()
	{
		// Arrange
		var services = new ServiceCollection();
		var config = BuildConfiguration();

		// Act
		services.AddIdentityInfrastructure(config);

		// Assert
		var provider = services.BuildServiceProvider();
		var schemes = provider.GetService<IAuthenticationSchemeProvider>();
		Assert.NotNull(schemes);
	}

	private IConfiguration BuildConfiguration()
	{
		var inMemorySettings = new Dictionary<string, string>
			{
				{"Jwt:Issuer", "TestIssuer"},
				{"Jwt:Audience", "TestAudience"},
				{"Jwt:Secret", "SuperSecretKey1234567890"}
			};

		return new ConfigurationBuilder()
			.AddInMemoryCollection(inMemorySettings)
			.Build();
	}
}
