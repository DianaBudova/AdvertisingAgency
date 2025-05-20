namespace AdvertisingAgency.BLL.Interfaces
{
    using AdvertisingAgency.BLL.DTOs;
    using System.Threading.Tasks;

    public interface IDiscountService
    {
        Task<DiscountDto> GetAsync(int id, CancellationToken ct);
        Task<IEnumerable<DiscountDto>> GetAllAsync(CancellationToken ct);
        Task<int> CreateAsync(CreateDiscountDto dto, int userId, CancellationToken ct);
        Task UpdateAsync(int id, UpdateDiscountDto dto, int userId, CancellationToken ct);
        Task DeleteAsync(int id, int userId, CancellationToken ct);
        Task<IEnumerable<DiscountDto>> GetActiveDiscountsAsync(CancellationToken ct = default);
    }
}