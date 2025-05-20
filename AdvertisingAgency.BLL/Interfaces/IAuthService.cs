namespace AdvertisingAgency.BLL.Interfaces
{
    using AdvertisingAgency.BLL.DTOs;
    using System.Threading.Tasks;

    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
        Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    }
}