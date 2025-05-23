namespace Б_121_23_2_ПІ_1_6.Models
{
    public class DiscountUpdateRequest
    {
        public int Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ServiceId { get; set; }
    }
}