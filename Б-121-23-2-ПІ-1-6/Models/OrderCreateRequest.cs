namespace Б_121_23_2_ПІ_1_6.Models
{
    public record OrderCreateRequest(int UserId, IEnumerable<OrderItemRequest> Items);
}
