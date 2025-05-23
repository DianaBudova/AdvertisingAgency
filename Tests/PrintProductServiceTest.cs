using Xunit;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.DAL.Entities;
using AdvertisingAgency.DAL.Abstractions;

namespace AdvertisingAgency.Tests.Services
{
    public class PrintProductServiceTests
    {
        // Імітація залежностей: UnitOfWork і AutoMapper
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PrintProductService _service;

        public PrintProductServiceTests()
        {
            // Ініціалізація моків
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            // Ініціалізація сервісу з передачею підроблених залежностей
            _service = new PrintProductService(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_WithTitleFilter_ReturnsFilteredResults()
        {
            // Arrange: створюється DTO з фільтром по назві
            var filter = new PrintProductFilterDto { Title = "Banner" };

            // Створення тестових даних (один з них відповідає фільтру, інший ні)
            var products = new List<PrintProduct>
            {
                new PrintProduct { Id = 1, Title = "Banner A", BaseCost = 10, Category = new Category { Name = "Outdoor" } },
                new PrintProduct { Id = 2, Title = "Flyer B", BaseCost = 5, Category = new Category { Name = "Indoor" } }
            };

            // Налаштування моку, який повертає список продуктів
            _mockUow.Setup(u => u.PrintProducts.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Мапінг сутності до DTO (імітується AutoMapper)
            var mappedProducts = new List<PrintProductDto>
            {
                new PrintProductDto(1, "Banner A", 10, "", "", "", "", 1)
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<PrintProductDto>>(It.IsAny<IEnumerable<PrintProduct>>()))
                .Returns(mappedProducts);

            // Act: виклик сервісного методу
            var result = await _service.GetAllAsync(filter);

            // Assert: перевірка, що результат містить лише один елемент з відповідною назвою
            Assert.Single(result);
            Assert.Contains(result, p => p.Title.Contains("Banner"));
        }

        [Fact]
        public async Task GetAsync_ExistingId_ReturnsProduct()
        {
            // Arrange: налаштування моку для повернення конкретного продукту за ID
            int productId = 1;
            var entity = new PrintProduct { Id = productId, Title = "Poster", BaseCost = 20, Category = new Category { Name = "Indoor" } };

            _mockUow.Setup(u => u.PrintProducts.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var dto = new PrintProductDto(1, "Poster", 20, "", "", "", "", 1);
            _mockMapper.Setup(m => m.Map<PrintProductDto>(entity)).Returns(dto);

            // Act: виклик GetAsync
            var result = await _service.GetAsync(productId);

            // Assert: перевірка, що результат відповідає очікуваним значенням
            Assert.Equal(productId, result.Id);
            Assert.Equal("Poster", result.Title);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsNewId()
        {
            // Arrange: створення вхідного DTO та користувача, який створює продукт
            var dto = new CreatePrintProductDto("Poster", 15, "Desc", "A4", "Glossy", "Color", 1);
            var user = new User { Id = 1, Role = new Role { Name = "Manager" } };

            // Налаштування моку для отримання користувача
            _mockUow.Setup(u => u.Users.GetByIdWithIncludesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Мапінг DTO до сутності
            var entity = new PrintProduct { Id = 10, Title = dto.Title, BaseCost = dto.BaseCost };
            _mockMapper.Setup(m => m.Map<PrintProduct>(dto)).Returns(entity);

            // Моки для додавання нового продукту та збереження змін
            _mockUow.Setup(u => u.PrintProducts.AddAsync(entity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act: створення продукту
            var resultId = await _service.CreateAsync(dto, 1);

            // Assert: перевірка, що повернене ID правильне
            Assert.Equal(10, resultId);
        }

        [Fact]
        public async Task DeleteAsync_ManagerUser_DeletesSuccessfully()
        {
            // Arrange: створення користувача з правами адміністратора та ID продукту, який видаляється
            int idToDelete = 5;
            var user = new User { Id = 1, Role = new Role { Name = "Administrator" } };

            _mockUow.Setup(u => u.Users.GetByIdWithIncludesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Налаштування видалення продукту
            _mockUow.Setup(u => u.PrintProducts.DeleteAsync(idToDelete, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act: виклик видалення
            await _service.DeleteAsync(idToDelete, user.Id);

            // Assert: перевірка, що метод видалення був викликаний один раз
            _mockUow.Verify(u => u.PrintProducts.DeleteAsync(idToDelete, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
