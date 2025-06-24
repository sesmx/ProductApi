using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ProductApi.Application.DTOs;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Application.Tests.Services;

public class ItemServiceTests
{
	private readonly Mock<IUnitOfWork> _uowMock;
	private readonly Mock<IMapper> _mapperMock;
	private readonly Mock<IValidator<AddItemDto>> _validatorMock;
	private readonly ItemService _service;

	public ItemServiceTests()
	{
		_uowMock = new Mock<IUnitOfWork>();
		_mapperMock = new Mock<IMapper>();
		_validatorMock = new Mock<IValidator<AddItemDto>>();
		_service = new ItemService(_uowMock.Object, _mapperMock.Object, _validatorMock.Object);
	}

	[Fact]
	public async Task RemoveItemAsync_ProductNotFound_ThrowsNotFoundException()
	{
		// Arrange
		var productId = 99;
		var itemId = 1;
		_uowMock.Setup(u => u.Products.GetAsync(productId, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Product)null);

		// Act & Assert
		await Assert.ThrowsAsync<NotFoundException>(() => _service.RemoveItemAsync(productId, itemId));
	}

	[Fact]
	public async Task RemoveItemAsync_ItemNotFound_ThrowsNotFoundException()
	{
		// Arrange
		var productId = 1;
		var itemId = 99;
		var product = Product.Create("Product Name", "user");

		_uowMock.Setup(u => u.Products.GetAsync(productId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		// Act & Assert
		await Assert.ThrowsAsync<NotFoundException>(() => _service.RemoveItemAsync(productId, itemId));
	}
}