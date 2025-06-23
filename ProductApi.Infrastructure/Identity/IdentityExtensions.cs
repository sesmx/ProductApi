using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Infrastructure.Data;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Infrastructure.Identity;

public static class IdentityExtensions
{
	public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection s, IConfiguration cfg)
	{
		s.Configure<JwtSettings>(cfg.GetSection("Jwt"));

		s.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		s.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		s.AddScoped<ICurrentUser, CurrentUserService>();

		s.AddIdentityCore<ApplicationUser>(options =>
		{
			// Password policy
			options.Password.RequireDigit = true; // at least one 0–9
			options.Password.RequireLowercase = true; // at least one a–z
			options.Password.RequireUppercase = true; // at least one A–Z
			options.Password.RequireNonAlphanumeric = false; // set true if we need symbols
			options.Password.RequiredLength = 8; // minimum length
			options.Password.RequiredUniqueChars = 1; // unique chars in the pwd

			// Lockout settings
			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // lock period
			options.Lockout.MaxFailedAccessAttempts = 5; // before lockout
			options.Lockout.AllowedForNewUsers = true;

			// User settings
			options.User.RequireUniqueEmail = true;
		})
		 .AddRoles<IdentityRole<int>>()
		 .AddEntityFrameworkStores<ApplicationDbContext>()
		 .AddSignInManager<SignInManager<ApplicationUser>>()
		 .AddDefaultTokenProviders();

		var jwt = cfg.GetSection("Jwt").Get<JwtSettings>()!;
		s.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		 .AddJwtBearer(opt =>
		 {
			 opt.TokenValidationParameters = new()
			 {
				 ValidateIssuer = true,
				 ValidateAudience = true,
				 ValidateLifetime = true,
				 ValidateIssuerSigningKey = true,
				 ValidIssuer = jwt.Issuer,
				 ValidAudience = jwt.Audience,
				 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
				 RoleClaimType = ClaimTypes.Role
			 };
		 });

		return s;
	}
}
