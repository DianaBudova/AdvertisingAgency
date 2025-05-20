namespace AdvertisingAgency.BLL.Interfaces;

using AdvertisingAgency.BLL.DTOs;
using System.Threading.Tasks;

public interface IPrintProductService
{
    Task<IEnumerable<PrintProductDto>> GetAllAsync(PrintProductFilterDto filter, CancellationToken ct = default);
    Task<PrintProductDto> GetAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(CreatePrintProductDto dto, int actorId, CancellationToken ct = default);
    Task UpdateAsync(int id, UpdatePrintProductDto dto, int actorId, CancellationToken ct = default);
    Task DeleteAsync(int id, int actorId, CancellationToken ct = default);
}