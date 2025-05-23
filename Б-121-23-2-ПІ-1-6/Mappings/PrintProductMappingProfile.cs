using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class PrintProductMappingProfile : Profile
    {
        public PrintProductMappingProfile()
        {
            // PL -> BLL
            CreateMap<PrintProductRequest, PrintProductFilterDto>();
            CreateMap<PrintProductCreateRequest, CreatePrintProductDto>();
            CreateMap<PrintProductUpdateRequest, UpdatePrintProductDto>();

            // BLL -> PL
            CreateMap<PrintProductDto, PrintProductResponse>();
        }
    }
}
