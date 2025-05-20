namespace AdvertisingAgency.BLL.DTOs
{
    public record OrderDto(int Id, int UserId, decimal Total, DateTime CreatedAt, string Status, IEnumerable<(int ServiceId, decimal Price, int Quantity)> Items);
}
