namespace AdvertisingAgency.BLL.DTOs
{
    public record UpdateServiceDto(string Name, string? Description, decimal Price, bool IsActive, int CategoryId);
}
