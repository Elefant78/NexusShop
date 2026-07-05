using FluentValidation;
using NexusShop.Application.Common.Exceptions;
using NexusShop.Application.Common.Mapping;
using NexusShop.Application.DTOs;
using NexusShop.Application.Interfaces;
using NexusShop.Domain.Entities;
using NexusShop.Domain.Interfaces;
using ValidationException = NexusShop.Application.Common.Exceptions.ValidationException;

namespace NexusShop.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryDto> _createValidator;

    public CategoryService(
        IUnitOfWork unitOfWork,
        IValidator<CreateCategoryDto> createValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.ListAllAsync(cancellationToken);
        return categories.ToDtoList();
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        return category.ToDto();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var category = new Category(dto.Name, dto.Description);

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }
}
