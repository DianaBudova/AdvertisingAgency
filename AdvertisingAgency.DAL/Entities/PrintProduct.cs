namespace AdvertisingAgency.DAL.Entities
{
    public class PrintProduct : BaseEntity
    {
        public string Title { get; set; } = null!;
        public decimal BaseCost { get; set; }
        public string Description { get; set; } = null!;
        public string Size { get; set; } = null!;         // формат, напр. A4, A5
        public string PaperType { get; set; } = null!;    // тип паперу, напр. глянцевий, матовий
        public string PrintType { get; set; } = null!;    // кольоровий, ч/б
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
