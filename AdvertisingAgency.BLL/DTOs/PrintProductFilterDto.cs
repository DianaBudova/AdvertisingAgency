namespace AdvertisingAgency.BLL.DTOs
{
    public class PrintProductFilterDto
    {
        public string? Title { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
