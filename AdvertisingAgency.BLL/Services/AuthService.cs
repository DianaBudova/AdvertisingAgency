namespace AdvertisingAgency.BLL.Services
{
    using AdvertisingAgency.BLL.DTOs;
    using AdvertisingAgency.BLL.Exceptions;
    using AdvertisingAgency.BLL.Interfaces;
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;
    using System.Text;
    using System.Security.Cryptography;
    using Microsoft.EntityFrameworkCore;

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;

        public AuthService(IUnitOfWork uow) => _uow = uow;

        public async Task<int> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            if (await EmailExists(dto.Email, ct))
                throw new ValidationException("Email already in use.");

            var hash = HashPassword(dto.Password);
            var user = new User { Email = dto.Email, PasswordHash = hash, RoleId = await GetRoleId("Registered", ct) };
            var id = await _uow.Users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
            return id;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var user = (await _uow.Users.GetAllAsync(ct))
                .FirstOrDefault(u => u.Email == dto.Email)
                ?? throw new ValidationException("Invalid credentials.");

            if (user.PasswordHash != HashPassword(dto.Password))
                throw new ValidationException("Invalid credentials.");

            var roleName = (await _uow.Roles.GetAllAsync(ct))
                      .FirstOrDefault(r => r.Id == user.RoleId)?.Name
                  ?? throw new ValidationException("User role not configured.");

            return new AuthResultDto(user.Id, user.Email, roleName);
        }

        private async Task<int> GetRoleId(string roleName, CancellationToken ct)
        {
            var role = (await _uow.Roles.GetAllAsync(ct)).FirstOrDefault(r => r.Name == roleName) ??
                       throw new ValidationException($"Role '{roleName}' not configured.");
            return role.Id;
        }

        private async Task<bool> EmailExists(string email, CancellationToken ct)
        {
            return (await _uow.Users.GetAllAsync(ct)).Any(u => u.Email == email);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}