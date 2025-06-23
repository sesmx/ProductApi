using AutoMapper;
using FluentValidation;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Application.Services;

public class ProductService : IProductService
{
	private readonly IUnitOfWork _uow;
	private readonly IMapper _mapper;
	private readonly IValidator<CreateProductDto> _createValidator;
	private readonly IValidator<UpdateProductDto> _updateValidator;

	public ProductService(IUnitOfWork uow,
		IMapper mapper,
		IValidator<CreateProductDto> createValidator,
		IValidator<UpdateProductDto> updateValidator)
	{
		_uow = uow;
		_mapper = mapper;
		_createValidator = createValidator;
		_updateValidator = updateValidator;
	}

	public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
	{
		var products = await _uow.Products.GetAllAsync(ct: ct);
		return _mapper.Map<IEnumerable<ProductDto>>(products);
	}

	public async Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default)
	{
		var entity = await _uow.Products.GetAsync(id, ct);
		if (entity is null) throw new NotFoundException(nameof(Product), id);
		return _mapper.Map<ProductDto>(entity);
	}

	public async Task<ProductDto> CreateAsync(CreateProductDto dto, string createdBy, CancellationToken ct = default)
	{
		await _createValidator.ValidateAndThrowAsync(dto, ct);

		var entity = Product.Create(dto.Name, createdBy);
		await _uow.Products.AddAsync(entity, ct);
		await _uow.SaveChangesAsync(ct);

		return _mapper.Map<ProductDto>(entity);
	}

	public async Task UpdateAsync(int id, UpdateProductDto dto, string updatedBy, CancellationToken ct = default)
	{
		await _updateValidator.ValidateAndThrowAsync(dto, ct);

		var entity = await _uow.Products.GetAsync(id, ct);
		if (entity is null) throw new NotFoundException(nameof(Product), id);

		entity.Rename(dto.Name, updatedBy);
		await _uow.SaveChangesAsync(ct);
	}

	public async Task DeleteAsync(int id, CancellationToken ct = default)
	{
		var entity = await _uow.Products.GetAsync(id, ct);
		if (entity is null) throw new NotFoundException(nameof(Product), id);

		_uow.Products.Remove(entity);
		await _uow.SaveChangesAsync(ct);
	}
}
