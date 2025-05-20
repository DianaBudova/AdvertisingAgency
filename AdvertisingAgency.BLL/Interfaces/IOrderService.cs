namespace AdvertisingAgency.BLL.Interfaces
{
    using AdvertisingAgency.BLL.DTOs;
    using System.Threading.Tasks;

    public interface IOrderService
    {
        Task<int> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId, CancellationToken ct = default);
        Task ChangeStatusAsync(int orderId, string status, int actorId, CancellationToken ct = default);
    }
}