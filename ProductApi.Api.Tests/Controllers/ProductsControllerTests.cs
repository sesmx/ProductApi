using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Api.Controllers;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using System.Security.Claims;

namespace ProductApi.Api.Tests.Controllers;

public class ProductsControllerTests
{
	private readonly Mock<IProductService> _mockProductService;
	private readonly Mock<IItemService> _mockItemService;
	private readonly ProductsController _controller;

	public ProductsControllerTests()
	{
		_mockProductService = new Mock<IProductService>();
		_mockItemService = new Mock<IItemService>();
		_controller = new ProductsController(_mockProductService.Object, _mockItemService.Object);

		var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
		{
			new Claim(ClaimTypes.Name, "TestUser")
		}, "mock"));

		_controller.ControllerContext = new ControllerContext
		{
			HttpContext = new DefaultHttpContext { User = user }
		};
	}

	[Fact]
	public async Task GetAll_ReturnsOkResult_WithListOfProducts()
	{
		// Arrange
		var products = new List<ProductDto>
		{
			new ProductDto(1, "Product 1", 20),
			new ProductDto(2, "Product 2", 30)
		};

		_mockProductService
			.Setup(svc => svc.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(products);

		// Act
		var result = await _controller.GetAll(CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.Equal(products, okResult.Value);
	}

	[Fact]
	public async Task GetById_ReturnsOkResult_WithProduct()
	{
		// Arrange
		int productId = 1;
		var product = new ProductDto(productId, "Product 1", 30);
		_mockProductService
			.Setup(svc => svc.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		// Act
		var result = await _controller.GetById(productId, CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.Equal(product, okResult.Value);
	}

	[Fact]
	public async Task Create_ReturnsCreatedAtActionResult_WithCreatedProduct()
	{
		// Arrange
		var createDto = new CreateProductDto("New Product");
		var createdProduct = new ProductDto(1, "New Product", 30);

		_mockProductService
			.Setup(svc => svc.CreateAsync(createDto, It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(createdProduct);

		// Act
		var result = await _controller.Create(createDto, CancellationToken.None);

		// Assert
		var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
		Assert.Equal(nameof(_controller.GetById), createdAtResult.ActionName);
		Assert.Equal(createdProduct, createdAtResult.Value);

		// Verify
		Assert.NotNull(createdAtResult.RouteValues);
		Assert.True(createdAtResult.RouteValues.ContainsKey("id"));
		Assert.Equal(createdProduct.Id, createdAtResult.RouteValues["id"]);
	}

	[Fact]
	public async Task Update_ReturnsNoContentResult()
	{
		// Arrange
		int productId = 1;
		var updateDto = new UpdateProductDto("Updated Product");

		_mockProductService
			.Setup(svc => svc.UpdateAsync(productId, updateDto, It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _controller.Update(productId, updateDto, CancellationToken.None);

		// Assert
		Assert.IsType<NoContentResult>(result);
		_mockProductService.Verify(
			svc => svc.UpdateAsync(productId, updateDto, It.IsAny<string>(), It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Fact]
	public async Task Delete_ReturnsNoContentResult()
	{
		// Arrange
		int productId = 1;
		_mockProductService
			.Setup(svc => svc.DeleteAsync(productId, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _controller.Delete(productId, CancellationToken.None);

		// Assert
		Assert.IsType<NoContentResult>(result);
		_mockProductService.Verify(
			svc => svc.DeleteAsync(productId, It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Fact]
	public async Task AddItem_ReturnsCreatedAtActionResult_WithCreatedItem()
	{
		// Arrange
		int productId = 1;

		var addItemDto = new AddItemDto(productId, 10);
		var createdItem = new ItemDto(1, productId, 10);

		_mockItemService
			.Setup(svc => svc.AddItemAsync(
				It.Is<AddItemDto>(dto => dto.ProductId == productId),
				It.IsAny<string>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(createdItem);

		// Act
		var result = await _controller.AddItem(productId, addItemDto, CancellationToken.None);

		// Assert
		var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
		Assert.Equal(nameof(_controller.GetItem), createdAtResult.ActionName);
		Assert.Equal(createdItem, createdAtResult.Value);

		// Verify
		Assert.NotNull(createdAtResult.RouteValues);
		Assert.True(createdAtResult.RouteValues.ContainsKey("productId"));
		Assert.True(createdAtResult.RouteValues.ContainsKey("itemId"));
		Assert.Equal(productId, createdAtResult.RouteValues["productId"]);
		Assert.Equal(createdItem.Id, createdAtResult.RouteValues["itemId"]);
	}
}