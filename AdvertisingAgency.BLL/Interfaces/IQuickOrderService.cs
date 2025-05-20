namespace AdvertisingAgency.BLL.Interfaces
{
    using AdvertisingAgency.BLL.DTOs;
    using System.Threading.Tasks;

    public interface IQuickOrderService
    {
        Task<int> CreateAsync(CreateQuickOrderDto dto, CancellationToken ct = default);
        Task<IEnumerable<QuickOrderDto>> GetUserOrdersAsync(string customerName, CancellationToken ct = default);
    }
}