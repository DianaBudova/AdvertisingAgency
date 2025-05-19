namespace AdvertisingAgency.DAL.Entities
{
    public class Discount : BaseEntity
    {
        public int Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    }
}
