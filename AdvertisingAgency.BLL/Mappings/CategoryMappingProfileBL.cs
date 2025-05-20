using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using AdvertisingAgency.DAL.Entities;

namespace AdvertisingAgency.BLL.Mapping
{
    public class CategoryMappingProfileBL : Profile
    {
        public CategoryMappingProfileBL()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
        }
    }
}
