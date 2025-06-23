using Microsoft.AspNetCore.Identity;
using ProductApi.Domain.Enums;

namespace ProductApi.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
	public string FullName { get; set; } = string.Empty;
	public string? ContactNumber { get; set; }
	public UserRole Role { get; set; } = UserRole.ProductUser;
}
