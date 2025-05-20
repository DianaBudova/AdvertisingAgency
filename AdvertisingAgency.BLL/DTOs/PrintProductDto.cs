namespace AdvertisingAgency.BLL.DTOs
{
    public record PrintProductDto(int Id, string Title, decimal BaseCost, string Description, string Size, string PaperType, string PrintType, int CategoryId);
}
