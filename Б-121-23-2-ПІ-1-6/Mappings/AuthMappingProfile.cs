using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // PL -> BLL
            CreateMap<RegisterRequest, RegisterDto>();
            CreateMap<LoginRequest, LoginDto>();
        }
    }
}
