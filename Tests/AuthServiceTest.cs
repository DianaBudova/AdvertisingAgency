using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Exceptions;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using Moq; 
using System.Threading;
using Xunit;

public class AuthServiceTest
{
    // Підроблені залежності
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IRepository<User>> _mockUserRepo;
    private readonly Mock<IRepository<Role>> _mockRoleRepo;
    private readonly AuthService _authService; // Сервіс, який тестується

    public AuthServiceTest()
    {
        // Ініціалізація моків (підробок)
        _mockUow = new Mock<IUnitOfWork>();
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockRoleRepo = new Mock<IRepository<Role>>();

        // Налаштування моків для повернення відповідних репозиторіїв
        _mockUow.SetupGet(u => u.Users).Returns(_mockUserRepo.Object);
        _mockUow.SetupGet(u => u.Roles).Returns(_mockRoleRepo.Object);

        // Ініціалізація сервісу з підробленими залежностями
        _authService = new AuthService(_mockUow.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldRegisterUser_WhenEmailDoesNotExist()
    {
        // Arrange: створення DTO для реєстрації
        var registerDto = new RegisterDto("test@email.com", "password123");

        // Імітація порожнього списку користувачів (емейл не існує)
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<User>());

        // Імітація наявності ролі "Registered"
        _mockRoleRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Role> { new Role { Id = 1, Name = "Registered" } });

        // Імітація успішного додавання користувача, що повертає його ID
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(1);

        // Act: виклик методу реєстрації
        var result = await _authService.RegisterAsync(registerDto);

        // Assert: перевірка, що користувач зареєстрований успішно
        Assert.Equal(1, result);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenEmailExists()
    {
        // Arrange: підготовка DTO з уже існуючим емейлом
        var registerDto = new RegisterDto("duplicate@email.com", "password123");

        // Імітація наявності користувача з таким емейлом
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<User> { new User { Email = "duplicate@email.com" } });

        // Assert: очікується виняток валідації при спробі реєстрації
        await Assert.ThrowsAsync<ValidationException>(() => _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResult_WhenCredentialsAreValid()
    {
        // Arrange: створення DTO для входу
        var loginDto = new LoginDto("valid@email.com", "password123");

        // Імітація правильного хешу паролю
        var passwordHash = Hash("password123");

        // Імітація користувача з вірними обліковими даними
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<User> {
                         new User { Id = 1, Email = "valid@email.com", PasswordHash = passwordHash, RoleId = 1 }
                     });

        // Імітація ролі
        _mockRoleRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Role> {
                         new Role { Id = 1, Name = "Registered" }
                     });

        // Act: виклик методу входу
        var result = await _authService.LoginAsync(loginDto);

        // Assert: перевірка, що результат авторизації правильний
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("valid@email.com", result.Email);
        Assert.Equal("Registered", result.Role);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowValidationException_WhenUserNotFound()
    {
        // Arrange: створення DTO користувача, якого не існує
        var loginDto = new LoginDto("notfound@email.com", "password123");

        // Імітація відсутності користувачів
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<User>());

        // Assert: очікується виняток при відсутності користувача
        await Assert.ThrowsAsync<ValidationException>(() => _authService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowValidationException_WhenPasswordIsWrong()
    {
        // Arrange: створення DTO з неправильним паролем
        var loginDto = new LoginDto("user@email.com", "wrongpassword");

        // Імітація правильного хешу (але не того паролю)
        var correctHash = Hash("correctpassword");

        // Імітація користувача з правильним емейлом, але іншим паролем
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<User> {
                         new User { Email = "user@email.com", PasswordHash = correctHash, RoleId = 1 }
                     });

        // Assert: очікується виняток через невірний пароль
        await Assert.ThrowsAsync<ValidationException>(() => _authService.LoginAsync(loginDto));
    }

    // Метод-хелпер для створення SHA256-хешу пароля
    private static string Hash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes); // Повернення хешу у вигляді рядка
    }
}
