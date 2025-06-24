using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using ProductApi.Application.DTOs;
using ProductApi.Application.Services;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Exceptions;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data.Repositories;

namespace ProductApi.Application.Tests.Services;

public class ProductServiceTests
{
	private readonly Mock<IUnitOfWork> _uowMock = new();
	private readonly Mock<IMapper> _mapperMock = new();
	private readonly Mock<IValidator<CreateProductDto>> _createValidatorMock = new();
	private readonly Mock<IValidator<UpdateProductDto>> _updateValidatorMock = new();
	private readonly Mock<IGenericRepository<Product>> _productRepoMock = new();
	private readonly ProductService _service;

	public ProductServiceTests()
	{
		_uowMock.SetupGet(u => u.Products).Returns(_productRepoMock.Object);
		_service = new ProductService(
			_uowMock.Object,
			_mapperMock.Object,
			_createValidatorMock.Object,
			_updateValidatorMock.Object
		);
	}

	[Fact]
	public async Task GetAllAsync_ReturnsMappedProducts()
	{
		// Arrange
		var products = new List<Product> { Product.Create("Test", "user") };
		var productDtos = new List<ProductDto> { new ProductDto(1, "Test", 10) };
		_productRepoMock.Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(products);
		_mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

		// Act
		var result = await _service.GetAllAsync();

		// Assert
		result.Should().BeEquivalentTo(productDtos);
	}

	[Fact]
	public async Task GetByIdAsync_ProductExists_ReturnsMappedProduct()
	{
		// Arrange
		var product = Product.Create("Test", "user");
		var productDto = new ProductDto(1, "Test", 10);
		_productRepoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
		_mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

		// Act
		var result = await _service.GetByIdAsync(1);

		// Assert
		result.Should().Be(productDto);
	}

	[Fact]
	public async Task GetByIdAsync_ProductNotFound_ThrowsNotFoundException()
	{
		// Arrange
		_productRepoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);

		// Act
		var act = async () => await _service.GetByIdAsync(1);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>();
	}

	[Fact]
	public async Task CreateAsync_ValidDto_AddsProductAndReturnsDto()
	{
		// Arrange
		var dto = new CreateProductDto("Test");

		var product = Product.Create("Test", "user");
		var productDto = new ProductDto(1, "Test", 10);

		// Use the actual interface method, not the extension method, for Moq setup
		_createValidatorMock
			.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
			.ReturnsAsync(new FluentValidation.Results.ValidationResult());

		_mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);

		_productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
		_uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

		// Act
		var result = await _service.CreateAsync(dto, "user");

		// Assert
		result.Should().Be(productDto);
		_productRepoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
		_uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_ProductNotFound_ThrowsNotFoundException()
	{
		// Arrange
		var dto = new UpdateProductDto("Updated");
		_updateValidatorMock
			.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
			.ReturnsAsync(new FluentValidation.Results.ValidationResult());
		_productRepoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);

		// Act
		var act = async () => await _service.UpdateAsync(1, dto, "user");

		// Assert
		await act.Should().ThrowAsync<NotFoundException>();
	}
	[Fact]
	public async Task DeleteAsync_ProductExists_RemovesProduct()
	{
		var product = Product.Create("Test", "user");
		_productRepoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
		_uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

		// Act
		await _service.DeleteAsync(1);

		// Assert
		_productRepoMock.Verify(r => r.Remove(product), Times.Once);
		_uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_ProductNotFound_ThrowsNotFoundException()
	{
		// Arrange
		_productRepoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Product)null);

		// Act
		var act = async () => await _service.DeleteAsync(1);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>();
	}
}