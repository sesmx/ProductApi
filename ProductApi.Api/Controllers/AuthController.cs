using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.Data.Configurations;
using ProductApi.Infrastructure.Identity;

namespace ProductApi.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
	private readonly SignInManager<ApplicationUser> _signIn;
	private readonly UserManager<ApplicationUser> _users;
	private readonly IJwtTokenGenerator _jwt;

	public AuthController(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> users, IJwtTokenGenerator jwt)
	{
		_signIn = signIn;
		_users = users;
		_jwt = jwt;
	}

	[HttpPost("login")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(TokenResponseDto), 200)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
	{
		var user = await _users.FindByNameAsync(dto.Username);
		if (user is null) return BadRequest("Invalid credentials.");

		var ok = await _signIn.CheckPasswordSignInAsync(user, dto.Password, false);
		if (!ok.Succeeded) return BadRequest("Invalid credentials.");

		var token = _jwt.Generate(user);

		return Ok(token);
	}

	[HttpPost("logout")]
	[Authorize]
	[ProducesResponseType(204)]
	public IActionResult Logout()
	{
		return NoContent();
	}
}
