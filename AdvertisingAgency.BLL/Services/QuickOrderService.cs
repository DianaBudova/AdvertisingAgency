using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;

namespace AdvertisingAgency.BLL.Services
{
    public class QuickOrderService : IQuickOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public QuickOrderService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateQuickOrderDto dto, CancellationToken ct = default)
        {
            var order = _mapper.Map<QuickOrder>(dto);
            order.CreatedAt = DateTime.UtcNow;

            await _uow.QuickOrders.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);

            return order.Id;
        }

        public async Task<IEnumerable<QuickOrderDto>> GetUserOrdersAsync(string customerName, CancellationToken ct = default)
        {
            var orders = (await _uow.QuickOrders.GetAllAsync(ct))
                .Where(order => order.CustomerName == customerName)
                .ToList();

            var orderDtos = _mapper.Map<IEnumerable<QuickOrderDto>>(orders);

            return orderDtos;
        }
    }
}
