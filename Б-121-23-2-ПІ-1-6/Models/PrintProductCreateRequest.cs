namespace Б_121_23_2_ПІ_1_6.Models
{
    public record PrintProductCreateRequest(string Title, decimal BaseCost, string Description, string Size, string PaperType, string PrintType, int CategoryId);
}
