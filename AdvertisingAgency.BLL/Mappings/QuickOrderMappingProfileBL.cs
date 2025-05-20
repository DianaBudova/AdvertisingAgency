using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;

namespace AdvertisingAgency.BLL.Mappings
{
    public class QuickOrderMappingProfileBL : Profile
    {
        public QuickOrderMappingProfileBL()
        {
            CreateMap<CreateQuickOrderDto, QuickOrder>();
            CreateMap<QuickOrder, QuickOrderDto>();
        }
    }
}
