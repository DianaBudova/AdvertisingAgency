namespace AdvertisingAgency.BLL.DTOs
{
    public record CreateServiceDto(string Name, string? Description, decimal Price, int CategoryId);
}
