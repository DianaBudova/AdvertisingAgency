namespace AdvertisingAgency.BLL.DTOs
{
    public class ServiceFilterDto
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
