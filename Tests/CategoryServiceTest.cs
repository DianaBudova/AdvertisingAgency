// Підключення необхідних просторів імен
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

public class CategoryServiceTests
{
    // Моки залежностей
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IRepository<Category>> _mockCategoryRepo;
    private readonly Mock<IRepository<User>> _mockUserRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        // Ініціалізація моків
        _mockUow = new Mock<IUnitOfWork>();
        _mockCategoryRepo = new Mock<IRepository<Category>>();
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockMapper = new Mock<IMapper>();

        // Підключення моків до UnitOfWork
        _mockUow.SetupGet(u => u.Categories).Returns(_mockCategoryRepo.Object);
        _mockUow.SetupGet(u => u.Users).Returns(_mockUserRepo.Object);

        // Ініціалізація сервісу категорій з моками
        _categoryService = new CategoryService(_mockUow.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCategory_WhenUserIsManagerAndNameIsUnique()
    {
        // Тест перевіряє, що категорія створюється, якщо ім’я унікальне і користувач має роль Manager
        var dto = new CreateCategoryDto("NewCategory");
        var userId = 1;
        var user = new User { Id = userId, Role = new Role { Name = "Manager" } };

        // Повертаємо користувача з роллю Manager
        _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        // Категорій з такою назвою ще немає
        _mockCategoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Category>());

        // Повертається ID новоствореної категорії
        _mockCategoryRepo.Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(1);

        var result = await _categoryService.CreateAsync(dto, userId);

        Assert.Equal(1, result); // Перевіряємо, що повернено правильний ID
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Збереження викликано
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCategoryNameAlreadyExists()
    {
        // Тест перевіряє, що виняток кидається, якщо така категорія вже існує
        var dto = new CreateCategoryDto("Existing");
        var user = new User { Id = 1, Role = new Role { Name = "Administrator" } };

        // Повертаємо користувача
        _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        // Імітуємо існуючу категорію з такою ж назвою
        _mockCategoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Category> { new Category { Name = "Existing" } });

        // Перевірка, що кидається ValidationException
        await Assert.ThrowsAsync<ValidationException>(() => _categoryService.CreateAsync(dto, 1));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenCategoryExistsAndUserIsManager()
    {
        // Тест перевіряє, що категорія успішно видаляється, якщо вона існує і користувач має роль Manager
        var category = new Category { Id = 1, Name = "ToDelete" };
        var user = new User { Id = 1, Role = new Role { Name = "Manager" } };

        _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(category);

        await _categoryService.DeleteAsync(1, 1);

        _mockCategoryRepo.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenCategoryDoesNotExist()
    {
        // Тест перевіряє, що якщо категорія не знайдена — кидається виняток EntityNotFoundException
        var user = new User { Id = 1, Role = new Role { Name = "Manager" } };

        _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        // Категорія не знайдена (null)
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _categoryService.DeleteAsync(99, 1));
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnMappedCategories()
    {
        // Тест перевіряє, що повертаються всі категорії в DTO-форматі
        var categories = new List<Category> { new Category { Id = 1, Name = "One" } };
        var categoryDtos = new List<CategoryDto> { new CategoryDto(1, "One") };

        _mockCategoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(categories);

        _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                   .Returns(categoryDtos);

        var result = await _categoryService.GetCategoriesAsync();

        Assert.Single(result);
        Assert.Equal("One", result.First().Name); // Перевіряємо правильність мапінгу
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenValidData()
    {
        // Тест перевіряє, що категорія оновлюється при валідних даних
        var category = new Category { Id = 1, Name = "Old" };
        var dto = new UpdateCategoryDto("Updated");
        var user = new User { Id = 1, Role = new Role { Name = "Administrator" } };

        _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(category);

        // Немає категорій з такою ж назвою, тому можна оновлювати
        _mockCategoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Category> { category });

        await _categoryService.UpdateAsync(1, dto, 1);

        _mockCategoryRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenNameAlreadyUsedByAnotherCategory()
    {
        // Тест перевіряє, що виняток кидається, якщо нова назва вже використовується іншою категорією
        var category = new Category { Id = 1, Name = "Original" };
        var dto = new UpdateCategoryDto("Conflict");
        var user = new User { Id = 1, Role = new Role { Name = "Administrator" } };

        _mockUserRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(category);

        // Окрім цієї категорії існує інша з назвою "Conflict"
        _mockCategoryRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Category> {
                             category,
                             new Category { Id = 2, Name = "Conflict" }
                         });

        await Assert.ThrowsAsync<ValidationException>(() => _categoryService.UpdateAsync(1, dto, 1));
    }
}
