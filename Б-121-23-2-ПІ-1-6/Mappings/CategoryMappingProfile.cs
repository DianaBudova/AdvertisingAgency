using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // PL -> BLL
            CreateMap<CategoryCreateRequest, CreateCategoryDto>();
            CreateMap<CategoryUpdateRequest, UpdateCategoryDto>();

            // BLL -> PL
            CreateMap<CategoryDto, CategoryResponse>();
        }
    }
}
