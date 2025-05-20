namespace AdvertisingAgency.BLL.DTOs
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public int Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ServiceId { get; set; }
        public ServiceDto Service { get; set; }
        public bool IsActive { get; set; }
    }
}