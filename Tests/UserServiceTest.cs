using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Exceptions;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AdvertisingAgency.Tests.Services
{
    public class UserServiceTest
    {
        // Моки для залежностей: UnitOfWork, Mapper, Репозиторій користувачів
        private readonly Mock<IUnitOfWork> _mockUow = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly Mock<IRepository<User>> _mockUserRepo = new();

        // Сервіс, який тестуємо
        private readonly UserService _userService;

        public UserServiceTest()
        {
            // Налаштовуємо UnitOfWork мок, щоб повертати мок репозиторію користувачів
            _mockUow.Setup(u => u.Users).Returns(_mockUserRepo.Object);

            // Ініціалізуємо сервіс UserService з моками
            _userService = new UserService(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange: створюємо тестову сутність користувача
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = "hashedpwd",
                RoleId = 2
            };

            // Очікуваний DTO, який має повернутися з сервісу
            var expectedDto = new UserDto(1, "test@example.com", "hashedpwd", 2);

            // Налаштовуємо мок репозиторію, щоб повертати користувача за id
            _mockUserRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Налаштовуємо мок мапера для конвертації сутності User в UserDto
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(expectedDto);

            // Act: викликаємо метод сервісу, який повинен повернути DTO користувача
            var result = await _userService.GetAsync(1);

            // Assert: перевіряємо, що результат співпадає з очікуваним DTO
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Email, result.Email);
            Assert.Equal(expectedDto.PasswordHash, result.PasswordHash);
            Assert.Equal(expectedDto.RoleId, result.RoleId);
        }

        [Fact]
        public async Task GetAsync_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange: налаштовуємо мок, щоб при пошуку неіснуючого користувача повертати null
            _mockUserRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((User?)null);

            // Act + Assert: перевіряємо, що метод викидає EntityNotFoundException, якщо користувача немає
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetAsync(999));
        }
    }
}
