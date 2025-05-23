using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class DiscountMappingProfile : Profile
    {
        public DiscountMappingProfile()
        {
            CreateMap<Discount, DiscountDto>();
            CreateMap<CreateDiscountDto, Discount>();
            CreateMap<UpdateDiscountDto, Discount>();
            CreateMap<DiscountDto, DiscountResponse>();
            CreateMap<DiscountCreateRequest, CreateDiscountDto>();
            CreateMap<DiscountUpdateRequest, UpdateDiscountDto>();
        }
    }
}
