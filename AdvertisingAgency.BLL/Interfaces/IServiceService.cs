namespace AdvertisingAgency.BLL.Interfaces
{
    using AdvertisingAgency.BLL.DTOs;
    using System.Threading.Tasks;

    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetAllAsync(ServiceFilterDto filter, CancellationToken ct = default);
        Task<ServiceDto> GetAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(CreateServiceDto dto, int actorId, CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateServiceDto dto, int actorId, CancellationToken ct = default);
        Task DeleteAsync(int id, int actorId, CancellationToken ct = default);
    }
}