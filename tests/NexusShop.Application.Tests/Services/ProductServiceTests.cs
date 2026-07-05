using FluentAssertions;
using Moq;
using NexusShop.Application.Common.Exceptions;
using NexusShop.Application.DTOs;
using NexusShop.Application.Services;
using NexusShop.Application.Tests.TestData;
using NexusShop.Application.Validators;
using NexusShop.Domain.Entities;
using NexusShop.Domain.Interfaces;
using Xunit;
using ValidationException = NexusShop.Application.Common.Exceptions.ValidationException;

namespace NexusShop.Application.Tests.Services;

public sealed class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _uow.SetupGet(u => u.Categories).Returns(_categories.Object);

        _sut = new ProductService(
            _uow.Object,
            new CreateProductDtoValidator(),
            new UpdateProductDtoValidator());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedProducts()
    {
        var products = new List<Product>
        {
            ProductTestFactory.CreateProduct("A", 10m),
            ProductTestFactory.CreateProduct("B", 20m)
        };
        _products.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().ContainInOrder("A", "B");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnDto()
    {
        var product = ProductTestFactory.CreateProduct("Headphones", 99.90m);
        _products.Setup(r => r.GetWithCategoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _sut.GetByIdAsync(1);

        result.Name.Should().Be("Headphones");
        result.Price.Should().Be(99.90m);
        result.Currency.Should().Be("CHF");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductMissing_ShouldThrowNotFound()
    {
        _products.Setup(r => r.GetWithCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _sut.GetByIdAsync(42);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnMappedProducts()
    {
        _products.Setup(r => r.ListByCategoryAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { ProductTestFactory.CreateProduct(categoryId: 5) });

        var result = await _sut.GetByCategoryAsync(5);

        result.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldPersistAndReturnDto()
    {
        var dto = new CreateProductDto
        {
            Name = "Keyboard",
            Description = "Mechanical",
            Price = 129.00m,
            Currency = "CHF",
            StockQuantity = 25,
            CategoryId = 1
        };
        _categories.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _products.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("Keyboard");
        result.Price.Should().Be(129.00m);
        result.StockQuantity.Should().Be(25);
        _products.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPrice_ShouldThrowValidationAndNotPersist()
    {
        var dto = new CreateProductDto
        {
            Name = "Bad",
            Price = 0m,           // invalid: must be > 0
            Currency = "CHF",
            StockQuantity = 1,
            CategoryId = 1
        };

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
        _categories.Verify(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _products.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryMissing_ShouldThrowNotFound()
    {
        var dto = new CreateProductDto
        {
            Name = "Phone",
            Price = 800m,
            Currency = "CHF",
            StockQuantity = 5,
            CategoryId = 99
        };
        _categories.Setup(r => r.ExistsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _products.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldApplyChanges()
    {
        var product = ProductTestFactory.CreateProduct("Old", 10m);
        _products.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var dto = new UpdateProductDto
        {
            Name = "New Name",
            Description = "Updated",
            Price = 49.99m,
            Currency = "CHF",
            StockQuantity = 7,
            IsActive = false
        };

        var result = await _sut.UpdateAsync(1, dto);

        result.Name.Should().Be("New Name");
        result.Price.Should().Be(49.99m);
        result.StockQuantity.Should().Be(7);
        result.IsActive.Should().BeFalse();
        _products.Verify(r => r.Update(product), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductMissing_ShouldThrowNotFound()
    {
        _products.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var dto = new UpdateProductDto { Name = "X", Price = 5m, Currency = "CHF", StockQuantity = 1 };

        var act = () => _sut.UpdateAsync(123, dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_ShouldThrowValidation()
    {
        var dto = new UpdateProductDto { Name = "", Price = -1m, Currency = "CHF", StockQuantity = 1 };

        var act = () => _sut.UpdateAsync(1, dto);

        await act.Should().ThrowAsync<ValidationException>();
        _products.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ShouldRemoveAndSave()
    {
        var product = ProductTestFactory.CreateProduct();
        _products.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _sut.DeleteAsync(1);

        _products.Verify(r => r.Remove(product), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductMissing_ShouldThrowNotFound()
    {
        _products.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = () => _sut.DeleteAsync(7);

        await act.Should().ThrowAsync<NotFoundException>();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
