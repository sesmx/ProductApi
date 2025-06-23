using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Domain.Enums;
using ProductApi.Infrastructure.Identity;

namespace ProductApi.Infrastructure.Data;

public static class IdentityDataSeeder
{
	public static async Task SeedAsync(this IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		if (users.Users.Any()) return;

		var admin = new ApplicationUser
		{
			UserName = "admin",
			Email = "admin@example.com",
			FullName = "System Admin",
			Role = UserRole.Admin,
			EmailConfirmed = true
		};

		admin.NormalizedUserName = admin.UserName.ToUpperInvariant();
		admin.NormalizedEmail = admin.Email.ToUpperInvariant();

		await users.CreateAsync(admin, "Dxfactor@123");

		var normal = new ApplicationUser
		{
			UserName = "user",
			Email = "user@example.com",
			FullName = "Normal User",
			Role = UserRole.ProductUser,
			EmailConfirmed = true
		};

		normal.NormalizedUserName = admin.UserName.ToUpperInvariant();
		normal.NormalizedEmail = admin.Email.ToUpperInvariant();

		await users.CreateAsync(normal, "Dxfactor@123");
	}
}
