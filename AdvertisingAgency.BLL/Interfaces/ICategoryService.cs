namespace AdvertisingAgency.BLL.Interfaces
{
    using AdvertisingAgency.BLL.DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICategoryService
    {
        Task<int> CreateAsync(CreateCategoryDto dto, int userId, CancellationToken ct = default);
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateCategoryDto dto, int userId, CancellationToken ct = default);
        Task DeleteAsync(int id, int userId, CancellationToken ct = default);
    }
}
