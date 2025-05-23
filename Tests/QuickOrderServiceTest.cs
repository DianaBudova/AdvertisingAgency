using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;
using Moq;
using Xunit;

namespace AdvertisingAgency.Tests.Services
{
    public class QuickOrderServiceTest
    {
        // Моки для залежностей сервісу
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<QuickOrder>> _mockQuickOrderRepo;
        private readonly QuickOrderService _service;

        public QuickOrderServiceTest()
        {
            // Ініціалізація моків
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockQuickOrderRepo = new Mock<IRepository<QuickOrder>>();

            // Підключення репозиторію замовлень до моканого UoW
            _mockUow.Setup(u => u.QuickOrders).Returns(_mockQuickOrderRepo.Object);

            // Ініціалізація сервісу з моками
            _service = new QuickOrderService(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddQuickOrderAndReturnId()
        {
            // Arrange: створення DTO для створення замовлення
            var dto = new CreateQuickOrderDto("John Doe", "123456789", 1);

            // Очікуваний об'єкт після мапінгу DTO -> Entity
            var quickOrder = new QuickOrder
            {
                Id = 42,
                CustomerName = dto.CustomerName,
                Phone = dto.Phone,
                ServiceId = dto.ServiceId
            };

            // Налаштування AutoMapper
            _mockMapper.Setup(m => m.Map<QuickOrder>(dto)).Returns(quickOrder);

            // Налаштування додавання замовлення через репозиторій
            _mockQuickOrderRepo.Setup(r => r.AddAsync(It.IsAny<QuickOrder>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync(quickOrder.Id);

            // Налаштування збереження змін
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

            // Act: виклик методу створення
            var result = await _service.CreateAsync(dto);

            // Assert: перевірка, що повертається правильний Id
            Assert.Equal(42, result);

            // Перевірка, що AddAsync був викликаний один раз з правильними даними
            _mockQuickOrderRepo.Verify(r => r.AddAsync(It.Is<QuickOrder>(q =>
                q.CustomerName == dto.CustomerName &&
                q.Phone == dto.Phone &&
                q.ServiceId == dto.ServiceId
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserOrdersAsync_ShouldReturnFilteredQuickOrders()
        {
            // Arrange: створення тестових замовлень, де лише частина належить користувачу "Alice"
            var customerName = "Alice";
            var quickOrders = new List<QuickOrder>
            {
                new QuickOrder { Id = 1, CustomerName = "Alice", Phone = "111", ServiceId = 1 },
                new QuickOrder { Id = 2, CustomerName = "Bob", Phone = "222", ServiceId = 2 },
                new QuickOrder { Id = 3, CustomerName = "Alice", Phone = "333", ServiceId = 3 }
            };

            // Очікувані DTO після фільтрації і мапінгу
            var expectedDtos = new List<QuickOrderDto>
            {
                new QuickOrderDto(1, "Alice", "111", 1),
                new QuickOrderDto(3, "Alice", "333", 3)
            };

            // Налаштування репозиторію для повернення всіх замовлень
            _mockQuickOrderRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(quickOrders);

            // Налаштування мапінгу лише для замовлень користувача "Alice"
            _mockMapper.Setup(m => m.Map<IEnumerable<QuickOrderDto>>(
                It.Is<IEnumerable<QuickOrder>>(q => q.All(o => o.CustomerName == "Alice"))
            )).Returns(expectedDtos);

            // Act: виклик методу отримання замовлень користувача
            var result = (await _service.GetUserOrdersAsync(customerName)).ToList();

            // Assert: перевірка кількості та вмісту результату
            Assert.Equal(2, result.Count);
            Assert.All(result, dto => Assert.Equal("Alice", dto.CustomerName));
        }
    }
}
