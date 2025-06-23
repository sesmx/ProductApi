using AutoMapper;
using FluentValidation;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Application.Services;

public class ItemService : IItemService
{
	private readonly IUnitOfWork _uow;
	private readonly IMapper _mapper;
	private readonly IValidator<AddItemDto> _addValidator;

	public ItemService(IUnitOfWork uow, IMapper mapper, IValidator<AddItemDto> addValidator)
	{
		_uow = uow;
		_mapper = mapper;
		_addValidator = addValidator;
	}

	public async Task<ItemDto> AddItemAsync(AddItemDto dto, string createdBy, CancellationToken ct = default)
	{
		await _addValidator.ValidateAndThrowAsync(dto, ct);

		var product = await _uow.Products.GetAsync(dto.ProductId, ct);
		if (product is null) throw new NotFoundException(nameof(Product), dto.ProductId);

		var item = product.AddItem(dto.Quantity, createdBy);
		await _uow.SaveChangesAsync(ct);

		return _mapper.Map<ItemDto>(item);
	}

	public async Task RemoveItemAsync(int productId, int itemId, CancellationToken ct = default)
	{
		var product = await _uow.Products.GetAsync(productId, ct);
		if (product is null) throw new NotFoundException(nameof(Product), productId);

		var item = product.Items.FirstOrDefault(i => i.Id == itemId);
		if (item is null) throw new NotFoundException(nameof(Item), itemId);

		product.Items.Remove(item);
		await _uow.SaveChangesAsync(ct);
	}
}
