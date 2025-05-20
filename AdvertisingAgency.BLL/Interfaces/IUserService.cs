using AdvertisingAgency.BLL.DTOs;

namespace AdvertisingAgency.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetAsync(int id, CancellationToken ct = default);
    }
}
