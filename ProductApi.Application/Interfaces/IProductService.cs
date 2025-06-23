using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface IProductService
{
	Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);
	Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default);
	Task<ProductDto> CreateAsync(CreateProductDto dto, string createdBy, CancellationToken ct = default);
	Task UpdateAsync(int id, UpdateProductDto dto, string updatedBy, CancellationToken ct = default);
	Task DeleteAsync(int id, CancellationToken ct = default);
}
