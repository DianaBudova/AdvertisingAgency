namespace Б_121_23_2_ПІ_1_6.Models
{
    public record ServiceUpdateRequest(string Name, string? Description, decimal Price, bool IsActive, int CategoryId);
}
