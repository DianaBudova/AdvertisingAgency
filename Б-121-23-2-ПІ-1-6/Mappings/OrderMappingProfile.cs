using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using Б_121_23_2_ПІ_1_6.Models;

namespace Б_121_23_2_ПІ_1_6.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // PL -> BLL
            CreateMap<OrderCreateRequest, CreateOrderDto>();
            CreateMap<OrderItemRequest, OrderItemDto>();

            // BLL -> PL
            CreateMap<OrderDto, OrderResponse>();
        }
    }
}
