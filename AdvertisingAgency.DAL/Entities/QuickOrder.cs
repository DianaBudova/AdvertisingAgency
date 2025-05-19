namespace AdvertisingAgency.DAL.Entities
{
    public class QuickOrder : BaseEntity
    {
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
