namespace Б_121_23_2_ПІ_1_6.Models
{
    public class PrintProductRequest
    {
        public string? Name {  get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
