using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Api.Controllers;
using ProductApi.Application.Interfaces;
using System.Security.Claims;

namespace ProductApi.Api.Tests.Controllers;

public class ItemsControllerTests
{
	private readonly Mock<IItemService> _itemServiceMock;
	private readonly ItemsController _controller;
	private readonly CancellationToken _cancellationToken;

	public ItemsControllerTests()
	{
		_itemServiceMock = new Mock<IItemService>();
		_controller = new ItemsController(_itemServiceMock.Object);
		_cancellationToken = CancellationToken.None;

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
	public async Task GetById_ReturnsNotFound()
	{
		// Arrange
		int productId = 1;
		int itemId = 1;

		// Act
		var result = await _controller.GetById(productId, itemId, _cancellationToken);

		// Assert
		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task Delete_CallsRemoveItemAsync_AndReturnsNoContent()
	{
		// Arrange
		int productId = 1;
		int itemId = 1;

		_itemServiceMock
			.Setup(s => s.RemoveItemAsync(productId, itemId, _cancellationToken))
			.Returns(Task.CompletedTask)
			.Verifiable();

		// Act
		var result = await _controller.Delete(productId, itemId, _cancellationToken);

		// Assert
		_itemServiceMock.Verify(s => s.RemoveItemAsync(productId, itemId, _cancellationToken), Times.Once);
		Assert.IsType<NoContentResult>(result);
	}
}