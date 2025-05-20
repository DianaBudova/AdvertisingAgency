namespace AdvertisingAgency.BLL.Mapping
{
    using AdvertisingAgency.BLL.DTOs;
    using AutoMapper;
    using AdvertisingAgency.DAL.Entities;

    public class UserMappingProfileBL : Profile
    {
        public UserMappingProfileBL()
        {
            CreateMap<User, UserDto>();
        }
    }
}