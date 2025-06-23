using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ProductApi.Infrastructure.Identity;

public interface ICurrentUser
{
	int? UserId { get; }
	bool IsAdmin { get; }
	bool IsAuth { get; }
}

public class CurrentUserService : ICurrentUser
{
	private readonly IHttpContextAccessor _ctx;
	public CurrentUserService(IHttpContextAccessor ctx) => _ctx = ctx;

	private ClaimsPrincipal? Principal => _ctx.HttpContext?.User;

	public int? UserId => int.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

	public bool IsAuth => Principal?.Identity?.IsAuthenticated ?? false;
	public bool IsAdmin => Principal?.IsInRole("Admin") ?? false;
}
