using NexusShop.Application.DTOs;

namespace NexusShop.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
}
