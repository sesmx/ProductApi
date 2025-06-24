using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data.Repositories;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Tests.Data.Repositories;

public class GenericRepositoryTests
{
	private readonly Mock<DbSet<Product>> _mockSet;
	private readonly Mock<ApplicationDbContext> _mockContext;
	private readonly GenericRepository<Product> _repository;

	public GenericRepositoryTests()
	{
		_mockSet = new Mock<DbSet<Product>>();
		_mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
		_mockContext.Setup(m => m.Set<Product>()).Returns(_mockSet.Object);
		_repository = new GenericRepository<Product>(_mockContext.Object);
	}

	[Fact]
	public async Task GetAsync_ReturnsEntity_WhenFound()
	{
		var product = Product.Create("Test", "TestUser");
		_mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		var result = await _repository.GetAsync(1);

		Assert.Equal(product, result);
	}

	[Fact]
	public async Task GetAsync_ReturnsNull_WhenNotFound()
	{
		_mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Product)null);

		var result = await _repository.GetAsync(99);

		Assert.Null(result);
	}

	[Fact]
	public async Task AddAsync_AddsEntity()
	{
		var product = Product.Create("Test", "TestUser");
		_mockSet.Setup(m => m.AddAsync(product, It.IsAny<CancellationToken>()))
			.ReturnsAsync((EntityEntry<Product>)null!);

		await _repository.AddAsync(product);

		_mockSet.Verify(m => m.AddAsync(product, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public void Remove_RemovesEntity()
	{
		var product = Product.Create("Test", "TestUser");

		_repository.Remove(product);

		_mockSet.Verify(m => m.Remove(product), Times.Once);
	}
}
