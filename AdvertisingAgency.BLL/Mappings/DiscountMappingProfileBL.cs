using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;

namespace AdvertisingAgency.BLL.Mappings
{
    public class DiscountMappingProfileBL : Profile
    {
        public DiscountMappingProfileBL()
        {
            CreateMap<Discount, DiscountDto>();
            CreateMap<CreateDiscountDto, Discount>();
        }
    }
}
