using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data.Repositories;
using Xunit;

namespace ProductApi.Infrastructure.Tests.Data.Repositories;

public class UnitOfWorkTests
{
	private readonly Mock<ApplicationDbContext> _dbContextMock;
	private readonly UnitOfWork _unitOfWork;

	public UnitOfWorkTests()
	{
		_dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
		_unitOfWork = new UnitOfWork(_dbContextMock.Object);
	}

	[Fact]
	public void Constructor_InitializesRepositories()
	{
		Assert.NotNull(_unitOfWork.Products);
		Assert.NotNull(_unitOfWork.Items);
		Assert.IsAssignableFrom<IGenericRepository<Product>>(_unitOfWork.Products);
		Assert.IsAssignableFrom<IGenericRepository<Item>>(_unitOfWork.Items);
	}

	[Fact]
	public async Task SaveChangesAsync_DelegatesToDbContext()
	{
		// Arrange
		var expected = 1;
		_dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(expected);

		// Act
		var result = await _unitOfWork.SaveChangesAsync();

		// Assert
		Assert.Equal(expected, result);
		_dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DisposeAsync_DelegatesToDbContext()
	{
		// Arrange
		var disposeCalled = false;
		_dbContextMock.Setup(db => db.DisposeAsync())
			.Callback(() => disposeCalled = true)
			.Returns(ValueTask.CompletedTask);

		// Act
		await _unitOfWork.DisposeAsync();

		// Assert
		Assert.True(disposeCalled);
		_dbContextMock.Verify(db => db.DisposeAsync(), Times.Once);
	}
}
