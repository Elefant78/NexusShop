using AutoMapper;
using FluentValidation;
using NexusShop.Application.Common.Exceptions;
using NexusShop.Application.DTOs;
using NexusShop.Application.Interfaces;
using NexusShop.Domain.Entities;
using NexusShop.Domain.Interfaces;
using NexusShop.Domain.ValueObjects;
using ValidationException = NexusShop.Application.Common.Exceptions.ValidationException;

namespace NexusShop.Application.Services;

/// <summary>
/// Implements the product use-cases. This class contains the business logic
/// that the test project covers to >80%. All persistence is delegated to the
/// IUnitOfWork abstraction, so the logic is fully unit-testable with mocks.
/// </summary>
public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.ListAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ProductDto>>(products);
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetWithCategoryAsync(id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.ListByCategoryAsync(categoryId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        // Business rule: the referenced category must exist.
        var categoryExists = await _unitOfWork.Categories.ExistsAsync(dto.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new NotFoundException(nameof(Category), dto.CategoryId);

        var product = new Product(
            dto.Name,
            dto.Description,
            new Money(dto.Price, dto.Currency),
            dto.StockQuantity,
            dto.CategoryId);

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        product.Rename(dto.Name);
        product.UpdateDescription(dto.Description);
        product.ChangePrice(new Money(dto.Price, dto.Currency));
        product.SetStock(dto.StockQuantity);

        if (dto.IsActive)
            product.Activate();
        else
            product.Deactivate();

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
