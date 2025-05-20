using AdvertisingAgency.BLL.DTOs;
using AutoMapper;
using AdvertisingAgency.DAL.Entities;

namespace AdvertisingAgency.BLL.Mapping
{
    public class OrderMappingProfileBL : Profile
    {
        public OrderMappingProfileBL()
        {
            CreateMap<OrderItemDto, OrderItem>();
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Items,
                    opt => opt.MapFrom(s => s.Items!.Select(i => new ValueTuple<int, decimal, int>(i.ServiceId, i.Price, i.Quantity)))
                );
        }
    }
}
