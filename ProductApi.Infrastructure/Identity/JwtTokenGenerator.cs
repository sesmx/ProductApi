using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Infrastructure.Data.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Infrastructure.Identity;

public interface IJwtTokenGenerator
{
	TokenResponseDto Generate(ApplicationUser user);
}

public class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly JwtSettings _cfg;
	private readonly byte[] _key;

	public JwtTokenGenerator(IOptions<JwtSettings> opts)
	{
		_cfg = opts.Value;
		_key = Encoding.UTF8.GetBytes(_cfg.Secret);
	}

	public TokenResponseDto Generate(ApplicationUser user)
	{
		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
			new(ClaimTypes.Role, user.Role.ToString())
		};

		var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);

		var expires = DateTime.UtcNow.AddMinutes(_cfg.AccessTokenMinutes);

		var token = new JwtSecurityToken(
			issuer: _cfg.Issuer,
			audience: _cfg.Audience,
			claims: claims,
			expires: expires,
			signingCredentials: creds);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);
		return new TokenResponseDto(jwt, expires);
	}
}
