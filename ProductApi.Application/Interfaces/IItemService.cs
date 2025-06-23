using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface IItemService
{
	Task<ItemDto> AddItemAsync(AddItemDto dto, string createdBy, CancellationToken ct = default);
	Task RemoveItemAsync(int productId, int itemId, CancellationToken ct = default);
}
