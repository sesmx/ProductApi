using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Services;
using ProductApi.Application.Validators;

namespace ProductApi.Application;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
	{
		// AutoMapper
		services.AddAutoMapper(typeof(Mapping.MappingProfile).Assembly);

		// Validators
		services.AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();
		services.AddScoped<IValidator<UpdateProductDto>, UpdateProductValidator>();
		services.AddScoped<IValidator<AddItemDto>, AddItemValidator>();

		// Services
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<IItemService, ItemService>();

		return services;
	}
}
