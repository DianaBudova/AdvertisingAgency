namespace AdvertisingAgency.BLL.DTOs;

public record CreatePrintProductDto(string Title, decimal BaseCost, string Description, string Size, string PaperType, string PrintType, int CategoryId);
