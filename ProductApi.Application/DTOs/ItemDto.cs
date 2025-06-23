namespace ProductApi.Application.DTOs;

public record ItemDto(int Id, int ProductId, int Quantity);

public record AddItemDto(int ProductId, int Quantity);
