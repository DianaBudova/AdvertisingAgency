using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using AdvertisingAgency.DAL.Entities;

namespace AdvertisingAgency.BLL.Mapping
{
    public class ServiceMappingProfileBL : Profile
    {
        public ServiceMappingProfileBL()
        {
            CreateMap<Service, ServiceDto>();
            CreateMap<CreateServiceDto, Service>();
            CreateMap<UpdateServiceDto, Service>();
        }
    }
}
