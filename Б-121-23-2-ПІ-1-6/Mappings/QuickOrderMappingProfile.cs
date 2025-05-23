using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class QuickOrderMappingProfile : Profile
    {
        public QuickOrderMappingProfile()
        {
            CreateMap<QuickOrderCreateRequest, CreateQuickOrderDto>();
        }
    }
}
