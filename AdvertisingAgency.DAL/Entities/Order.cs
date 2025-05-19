namespace AdvertisingAgency.DAL.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public ICollection<OrderItem>? Items { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.New;
    }
}
