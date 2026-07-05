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

public sealed class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _uow.SetupGet(u => u.Categories).Returns(_categories.Object);
        _sut = new CategoryService(_uow.Object, new CreateCategoryDtoValidator());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedCategories()
    {
        _categories.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { ProductTestFactory.CreateCategory("Books") });

        var result = await _sut.GetAllAsync();

        result.Should().ContainSingle(c => c.Name == "Books");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ShouldThrowNotFound()
    {
        _categories.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var act = () => _sut.GetByIdAsync(10);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldPersist()
    {
        var dto = new CreateCategoryDto { Name = "Toys", Description = "Fun stuff" };
        _categories.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("Toys");
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ShouldThrowValidation()
    {
        var dto = new CreateCategoryDto { Name = "" };

        var act = () => _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
