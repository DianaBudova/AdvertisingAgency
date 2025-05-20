namespace AdvertisingAgency.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AdvertisingAgency.BLL.DTOs;
    using AdvertisingAgency.BLL.Exceptions;
    using AdvertisingAgency.BLL.Interfaces;
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;

        public OrderService(IUnitOfWork uow, IMapper mapper, IDiscountService discountService)
        {
            _uow = uow;
            _mapper = mapper;
            _discountService = discountService;
        }

        public async Task<int> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
        {
            await EnsureManagerRights(dto.UserId, ct);

            if (!dto.Items.Any())
                throw new ValidationException("Order must contain at least one service.");

            decimal total = 0m;
            var orderItems = new List<OrderItem>();

            // Get active discounts once
            var activeDiscounts = (await _discountService.GetActiveDiscountsAsync(ct)).ToList();

            foreach (var itemDto in dto.Items)
            {
                var service = await _uow.Services.GetByIdAsync(itemDto.ServiceId, ct) ??
                            throw new EntityNotFoundException(nameof(Service), itemDto.ServiceId);

                // Find applicable discount for this service
                var discount = activeDiscounts.FirstOrDefault(d => d.ServiceId == service.Id);
                var price = CalculateDiscountedPrice(service.Price, discount) * itemDto.Quantity;

                total += price;
                orderItems.Add(new OrderItem
                {
                    ServiceId = service.Id,
                    Price = price / itemDto.Quantity,
                    Quantity = itemDto.Quantity
                });
            }

            var order = new Order
            {
                UserId = dto.UserId,
                Total = total,
                Items = orderItems,
                Status = OrderStatus.InProgress
            };

            var id = await _uow.Orders.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
            return id;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId, CancellationToken ct = default)
        {
            var orders = (await _uow.Orders.GetAllAsync(ct))
                .Where(o => o.UserId == userId)
                .ToList();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderDetailsAsync(int orderId, int userId, CancellationToken ct = default)
        {
            var order = await _uow.Orders.GetByIdWithIncludesAsync(orderId, ct) ??
                       throw new EntityNotFoundException(nameof(Order), orderId);

            // Only allow access to own orders unless admin/manager
            var user = await _uow.Users.GetByIdWithIncludesAsync(userId, ct);
            if (user.Role?.Name == "registered" && order.UserId != userId)
                throw new ForbiddenException("Cannot view others' orders.");

            return _mapper.Map<OrderDto>(order);
        }

        public async Task ChangeStatusAsync(int orderId, string status, int actorId, CancellationToken ct = default)
        {
            var order = await _uow.Orders.GetByIdWithIncludesAsync(orderId, ct) ??
                       throw new EntityNotFoundException(nameof(Order), orderId);

            var actor = await _uow.Users.GetByIdWithIncludesAsync(actorId, ct) ??
                       throw new ForbiddenException("User not found.");

            if (actor.Role?.Name == "Registered" && order.UserId != actorId)
                throw new ForbiddenException("Cannot change status of others' orders.");

            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                throw new ValidationException("Invalid status value.");

            order.Status = newStatus;
            await _uow.Orders.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ApplyDiscountToOrderAsync(int orderId, int discountId, int actorId, CancellationToken ct = default)
        {
            var order = await _uow.Orders.GetByIdWithIncludesAsync(orderId, ct) ??
                       throw new EntityNotFoundException(nameof(Order), orderId);

            if (order.Status != OrderStatus.InProgress)
                throw new ValidationException("Can only apply discounts to pending orders.");

            var discount = await _uow.Discounts.GetByIdAsync(discountId, ct) ??
                         throw new EntityNotFoundException(nameof(Discount), discountId);

            if (DateTime.UtcNow < discount.StartDate || DateTime.UtcNow > discount.EndDate)
                throw new ValidationException("Discount is not active.");

            decimal newTotal = 0m;
            foreach (var item in order.Items.Where(i => i.ServiceId == discount.ServiceId))
            {
                item.Price = CalculateDiscountedPrice(item.Price, discount);
                newTotal += item.Price * item.Quantity;
            }

            newTotal += order.Items
                .Where(i => i.ServiceId != discount.ServiceId)
                .Sum(i => i.Price * i.Quantity);

            order.Total = newTotal;
            await _uow.Orders.UpdateAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private decimal CalculateDiscountedPrice(decimal originalPrice, DiscountDto discount)
        {
            if (discount == null || !discount.IsActive)
                return originalPrice;

            return originalPrice * (100 - discount.Percentage) / 100;
        }

        private decimal CalculateDiscountedPrice(decimal originalPrice, Discount discount)
        {
            if (discount == null || !discount.IsActive)
                return originalPrice;

            return originalPrice * (100 - discount.Percentage) / 100;
        }

        private async Task EnsureManagerRights(int userId, CancellationToken ct)
        {
            var user = await _uow.Users.GetByIdWithIncludesAsync(userId, ct) ??
                      throw new ForbiddenException("User not found.");
            if (user.Role == null || user.Role.Name is not ("Registered"))
                throw new ForbiddenException("Only registered user can orders.");
        }
    }
}