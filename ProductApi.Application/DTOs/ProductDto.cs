namespace ProductApi.Application.DTOs;

public record ProductDto(int Id, string Name, int TotalItems);

public record CreateProductDto(string Name);
public record UpdateProductDto(string Name);
