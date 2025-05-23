using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Exceptions;
using AdvertisingAgency.BLL.Interfaces;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;
using Moq;
using Xunit;

namespace AdvertisingAgency.Tests.Services
{
    public class OrderServiceTests
    {
        // Моки залежностей
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDiscountService> _discountServiceMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _discountServiceMock = new Mock<IDiscountService>();

            // Ініціалізація OrderService з моками
            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _discountServiceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenNoItems()
        {
            // Arrange: створюємо замовлення без товарів
            var dto = new CreateOrderDto(1, new List<OrderItemDto>());

            // Налаштовуємо роль користувача
            SetupUserWithRole(1, "Registered");

            // Act & Assert: очікуємо помилку валідації через відсутність товарів
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _orderService.CreateAsync(dto));
            Assert.Equal("Order must contain at least one service.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateOrder_WhenValid()
        {
            // Arrange: створюємо замовлення з двома товарами
            var items = new List<OrderItemDto>
            {
                new OrderItemDto(1, 2),  // 2 одиниці сервісу 1
                new OrderItemDto(2, 1)   // 1 одиниця сервісу 2
            };
            var dto = new CreateOrderDto(1, items);
            SetupUserWithRole(1, "Registered");

            // Список сервісів, які відповідають ID
            var services = new List<Service>
            {
                new Service { Id = 1, Price = 100m },
                new Service { Id = 2, Price = 200m }
            };

            // Мок: повернення сервісу по ID
            _unitOfWorkMock.Setup(u => u.Services.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int id, CancellationToken ct) => services.FirstOrDefault(s => s.Id == id));

            // Мок: повернення порожнього списку знижок
            _discountServiceMock.Setup(d => d.GetActiveDiscountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DiscountDto>());

            // Мок: повернення ID нового замовлення
            _unitOfWorkMock.Setup(u => u.Orders.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(42);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act: створення замовлення
            var resultId = await _orderService.CreateAsync(dto);

            // Assert: перевірка правильності створення
            Assert.Equal(42, resultId);
            _unitOfWorkMock.Verify(u => u.Orders.AddAsync(It.Is<Order>(o =>
                o.UserId == 1 &&
                o.Items.Count == 2 &&
                o.Total == 100m * 2 + 200m * 1), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_ShouldThrow_WhenInvalidStatus()
        {
            // Arrange: налаштування замовлення з коректним статусом і користувачем
            int orderId = 10;
            int actorId = 5;

            SetupUserWithRole(actorId, "Registered");

            var order = new Order { Id = orderId, UserId = actorId, Status = OrderStatus.New };

            _unitOfWorkMock.Setup(u => u.Orders.GetByIdWithIncludesAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _unitOfWorkMock.Setup(u => u.Users.GetByIdWithIncludesAsync(actorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = actorId, Role = new Role { Name = "Registered" } });

            // Act & Assert: очікуємо помилку при передачі невалідного статусу
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _orderService.ChangeStatusAsync(orderId, "InvalidStatus", actorId));

            Assert.Equal("Invalid status value.", ex.Message);
        }

        // Допоміжний метод: повертає користувача з певною роллю
        private void SetupUserWithRole(int userId, string roleName)
        {
            var user = new User
            {
                Id = userId,
                Role = new Role { Name = roleName }
            };

            _unitOfWorkMock.Setup(u => u.Users.GetByIdWithIncludesAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
        }
    }
}
