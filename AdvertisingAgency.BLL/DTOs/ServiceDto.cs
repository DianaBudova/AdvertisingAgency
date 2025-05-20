namespace AdvertisingAgency.BLL.DTOs
{
    public record ServiceDto(int Id, string Name, string? Description, decimal Price, bool IsActive, int CategoryId);
}
