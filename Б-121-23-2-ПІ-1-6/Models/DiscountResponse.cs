namespace Б_121_23_2_ПІ_1_6.Models
{
    public class DiscountResponse
    {
        public int Id { get; set; }
        public int Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ServiceId { get; set; }
        public ServiceResponse Service { get; set; }
        public bool IsActive { get; set; }
    }
}