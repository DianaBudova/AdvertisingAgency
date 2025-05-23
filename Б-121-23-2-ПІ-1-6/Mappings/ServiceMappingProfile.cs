using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            // PL -> BLL
            CreateMap<ServiceRequest, ServiceFilterDto>();
            CreateMap<ServiceCreateRequest, CreateServiceDto>();
            CreateMap<ServiceUpdateRequest, UpdateServiceDto>();

            // BLL -> PL
            CreateMap<ServiceDto, ServiceResponse>();
        }
    }
}
