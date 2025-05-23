namespace Б_121_23_2_ПІ_1_6.Models
{
    public record OrderResponse(int Id, int UserId, decimal Total, DateTime CreatedAt, string Status, IEnumerable<(int ServiceId, decimal Price, int Quantity)> Items);
}