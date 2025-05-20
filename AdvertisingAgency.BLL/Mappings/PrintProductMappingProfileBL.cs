using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;

namespace AdvertisingAgency.BLL.Mappings
{
    public class PrintProductMappingProfileBL : Profile
    {
        public PrintProductMappingProfileBL()
        {
            CreateMap<PrintProduct, PrintProductDto>();
            CreateMap<CreatePrintProductDto, PrintProduct>();
            CreateMap<UpdatePrintProductDto, PrintProduct>();
        }
    }
}
