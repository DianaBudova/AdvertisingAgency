namespace AdvertisingAgency.DAL.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
