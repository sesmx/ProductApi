using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Identity;
using ProductApi.Infrastructure.Logging;
using ProductApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
builder.Services.AddApplicationLayer()
	.AddStructuredLogging(builder.Configuration)
	   .AddIdentityInfrastructure(builder.Configuration)
	   .AddInfrastructureServices();
builder.Services.AddTransient<RequestLoggingMiddleware>();

// DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
	opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sql => sql.EnableRetryOnFailure()));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// API versioning
builder.Services.AddApiVersioning(o =>
{
	o.AssumeDefaultVersionWhenUnspecified = true;
	o.DefaultApiVersion = new ApiVersion(1, 0);
	o.ReportApiVersions = true;
});

// Configure CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin",
		builder => builder
			.AllowAnyHeader()
			.AllowAnyMethod()
			.SetIsOriginAllowed(origin => true)
			.AllowCredentials()
			.WithExposedHeaders("Content-Disposition", "X-Message", "X-FileName"));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new() { Title = "Product API", Version = "v1" });

	var scheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Description = "Paste the JWT after logging in, without the Bearer prefix."
	};
	c.AddSecurityDefinition("Bearer", scheme);
	c.AddSecurityRequirement(new()
	{
		[scheme] = Array.Empty<string>()
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowSpecificOrigin");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.Services.SeedAsync();

app.Run();
