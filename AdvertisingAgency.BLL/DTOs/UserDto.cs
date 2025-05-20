namespace AdvertisingAgency.BLL.DTOs
{
    public record UserDto(int Id, string Email, string PasswordHash, int RoleId);
}
