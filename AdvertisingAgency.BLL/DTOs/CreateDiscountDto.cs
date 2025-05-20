namespace AdvertisingAgency.BLL.DTOs
{
    public class CreateDiscountDto
    {
        public int Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ServiceId { get; set; }
    }
}