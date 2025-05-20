namespace AdvertisingAgency.BLL.DTOs
{
    public record CreateOrderDto(int UserId, IEnumerable<OrderItemDto> Items);
}
