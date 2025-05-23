using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Exceptions;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AdvertisingAgency.Tests.Services
{
    public class ServiceServiceTest
    {
        // Моки для підключення до залежностей (UnitOfWork, Mapper, Репозиторії)
        private readonly Mock<IUnitOfWork> _mockUow = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<IRepository<Service>> _mockServiceRepo = new();
        private readonly Mock<IRepository<User>> _mockUserRepo = new();

        // Сервіс, який тестуємо
        private readonly ServiceService _service;

        public ServiceServiceTest()
        {
            // Налаштовуємо моки UnitOfWork, щоб повертали відповідні моки репозиторіїв
            _mockUow.Setup(u => u.Services).Returns(_mockServiceRepo.Object);
            _mockUow.Setup(u => u.Users).Returns(_mockUserRepo.Object);

            // Ініціалізуємо сервіс, передаючи моки
            _service = new ServiceService(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredServices()
        {
            // Arrange: створюємо тестові дані сервісів
            var services = new List<Service>
            {
                new() { Id = 1, Name = "Banner", Price = 100, Category = new Category { Name = "Design" } },
                new() { Id = 2, Name = "Flyer", Price = 50, Category = new Category { Name = "Print" } }
            };

            // Налаштовуємо мок репозиторію, щоб повертати тестові сервіси
            _mockServiceRepo.Setup(r => r.GetAllWithIncludesAsync(It.IsAny<CancellationToken>()))
                            .ReturnsAsync(services);

            // Фільтр для пошуку сервісів
            var filter = new ServiceFilterDto { Name = "Ban", Category = "Des", MinPrice = 90, MaxPrice = 110 };

            // Мок для мапінгу сутностей Service у DTO
            _mockMapper.Setup(m => m.Map<IEnumerable<ServiceDto>>(It.IsAny<IEnumerable<Service>>()))
                       .Returns(new List<ServiceDto> { new(1, "Banner", null, 100, true, 1) });

            // Act: викликаємо метод GetAllAsync з фільтром
            var result = (await _service.GetAllAsync(filter)).ToList();

            // Assert: перевіряємо, що повернувся один сервіс з ім'ям "Banner"
            Assert.Single(result);
            Assert.Equal("Banner", result[0].Name);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnServiceDto_WhenExists()
        {
            // Arrange: створюємо тестовий сервіс та відповідний DTO
            var service = new Service { Id = 1, Name = "Banner" };
            var dto = new ServiceDto(1, "Banner", null, 100, true, 1);

            // Налаштовуємо мок репозиторію для повернення сервісу за id
            _mockServiceRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(service);

            // Налаштовуємо мок мапера для конвертації в DTO
            _mockMapper.Setup(m => m.Map<ServiceDto>(service)).Returns(dto);

            // Act: отримуємо сервіс за id
            var result = await _service.GetAsync(1);

            // Assert: перевіряємо, що отриманий DTO відповідає очікуванням
            Assert.Equal(1, result.Id);
            Assert.Equal("Banner", result.Name);
        }

        [Fact]
        public async Task GetAsync_ShouldThrow_WhenNotFound()
        {
            // Arrange: мок репозиторію повертає null, якщо сервіс не знайдений
            _mockServiceRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Service?)null);

            // Act + Assert: перевіряємо, що метод викидає EntityNotFoundException при відсутності сервісу
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetAsync(1));
        }

        [Fact]
        public async Task CreateAsync_ShouldAddService_WhenActorIsManager()
        {
            // Arrange: готуємо DTO для створення сервісу
            var dto = new CreateServiceDto("Banner", "Desc", 100, 1);

            // Створюємо сутність Service, яка має повернутися після створення
            var service = new Service { Id = 5, Name = dto.Name };

            // Користувач з роллю Manager (для авторизації)
            var user = new User { Id = 10, Role = new Role { Name = "Manager" } };

            // Моки для отримання користувача та мапінгу DTO у сутність
            _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(10, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<Service>(dto)).Returns(service);

            // Мок для додавання сервісу та збереження змін
            _mockServiceRepo.Setup(r => r.AddAsync(service, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act: викликаємо метод створення сервісу
            var result = await _service.CreateAsync(dto, 10);

            // Assert: перевіряємо, що метод повернув правильний id нового сервісу
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateService_WhenAuthorized()
        {
            // Arrange: існуючий сервіс та DTO оновлення
            var existingService = new Service { Id = 1, Name = "OldName" };
            var updateDto = new UpdateServiceDto("NewName", null, 120, true, 1);

            // Користувач з роллю Administrator
            var user = new User { Id = 20, Role = new Role { Name = "Administrator" } };

            // Моки для отримання користувача і сервісу
            _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(20, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(user);
            _mockServiceRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(existingService);

            // Act: оновлення сервісу
            await _service.UpdateAsync(1, updateDto, 20);

            // Assert: перевіряємо, що мапінг і оновлення викликались один раз, а зміни збережені
            _mockMapper.Verify(m => m.Map(updateDto, existingService), Times.Once);
            _mockServiceRepo.Verify(r => r.UpdateAsync(existingService, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete_WhenAuthorized()
        {
            // Arrange: користувач з роллю Manager
            var user = new User { Id = 99, Role = new Role { Name = "Manager" } };
            _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(99, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(user);

            // Act: видалення сервісу
            await _service.DeleteAsync(123, 99);

            // Assert: перевірка, що метод DeleteAsync та SaveChanges викликались
            _mockServiceRepo.Verify(r => r.DeleteAsync(123, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EnsureManagerRights_ShouldThrow_WhenUserNotManager()
        {
            // Arrange: користувач з роллю Customer (не Manager)
            var user = new User { Id = 1, Role = new Role { Name = "Customer" } };
            _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(user);

            // Act + Assert: перевіряємо, що виклик методу DeleteAsync викидає ForbiddenException
            await Assert.ThrowsAsync<ForbiddenException>(() => _service.DeleteAsync(1, 1));
        }
    }
}
