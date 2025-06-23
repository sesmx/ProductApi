using AutoMapper;
using ProductApi.Application.DTOs;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.Mapping;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Product, ProductDto>()
			.ForCtorParam("Id",
				opt => opt.MapFrom(src => src.Id))
			.ForCtorParam("Name",
				opt => opt.MapFrom(src => src.ProductName))
			.ForCtorParam("TotalItems",
				opt => opt.MapFrom(src => src.Items.Count));

		CreateMap<Item, ItemDto>();
	}
}
