using AdvertisingAgency.BLL.DTOs;
using AdvertisingAgency.BLL.Services;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Entities;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class DiscountServiceTests
{
    // Моки для залежностей
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IRepository<Discount>> _mockDiscountRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DiscountService _discountService;

    public DiscountServiceTests()
    {
        // Ініціалізація моків
        _mockUow = new Mock<IUnitOfWork>();
        _mockDiscountRepo = new Mock<IRepository<Discount>>();
        _mockMapper = new Mock<IMapper>();

        // Повернення мокованого репозиторію знижок
        _mockUow.Setup(u => u.Discounts).Returns(_mockDiscountRepo.Object);

        // Ініціалізація сервісу зі змоканими залежностями
        _discountService = new DiscountService(_mockUow.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnMappedDiscount_WhenExists()
    {
        // Arrange: створення знижки та DTO
        var discount = new Discount { Id = 1, Percentage = 10 };
        var dto = new DiscountDto { Id = 1, Percentage = 10 };

        // Налаштування моку для отримання знижки та мапінгу
        _mockDiscountRepo.Setup(r => r.GetByIdWithIncludesAsync(1, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(discount);
        _mockMapper.Setup(m => m.Map<DiscountDto>(discount)).Returns(dto);

        // Act: виклик GetAsync
        var result = await _discountService.GetAsync(1, CancellationToken.None);

        // Assert: перевірка результатів
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.Percentage);
    }

    [Fact]
    public async Task GetAsync_ShouldThrow_WhenNotFound()
    {
        // Arrange: Повернення null, якщо знижку не знайдено
        _mockDiscountRepo.Setup(r => r.GetByIdWithIncludesAsync(999, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Discount)null);

        // Act & Assert: перевірка на виключення
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _discountService.GetAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDiscounts()
    {
        // Arrange: створення списку знижок та відповідного DTO
        var discounts = new List<Discount> { new Discount { Id = 1, Percentage = 15 } };
        var dtos = new List<DiscountDto> { new DiscountDto { Id = 1, Percentage = 15 } };

        // Налаштування моку
        _mockDiscountRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(discounts);
        _mockMapper.Setup(m => m.Map<IEnumerable<DiscountDto>>(discounts)).Returns(dtos);

        // Act
        var result = await _discountService.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(15, result.First().Percentage);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAndReturnId()
    {
        // Arrange: створення DTO для нової знижки та відповідного ентіті
        var dto = new CreateDiscountDto { Percentage = 20, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), ServiceId = 1 };
        var discount = new Discount { Id = 123, Percentage = 20 };

        // Налаштування мапера та репозиторію
        _mockMapper.Setup(m => m.Map<Discount>(dto)).Returns(discount);
        _mockDiscountRepo.Setup(r => r.AddAsync(discount, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(123);
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

        // Act
        var result = await _discountService.CreateAsync(dto, 1, CancellationToken.None);

        // Assert
        Assert.Equal(123, discount.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldMapAndUpdate_WhenExists()
    {
        // Arrange: існуюча знижка та DTO для оновлення
        var discount = new Discount { Id = 1, Percentage = 10 };
        var dto = new UpdateDiscountDto { Percentage = 25, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), ServiceId = 2 };

        // Налаштування мапера та репозиторію
        _mockDiscountRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(discount);
        _mockMapper.Setup(m => m.Map(dto, discount)); // оновлення існуючого об'єкта
        _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _discountService.UpdateAsync(1, dto, 1, CancellationToken.None);

        // Assert: перевірка, що метод оновлення був викликаний
        _mockDiscountRepo.Verify(r => r.UpdateAsync(discount, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenDiscountNotFound()
    {
        // Arrange: знижку не знайдено
        _mockDiscountRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Discount)null);

        var dto = new UpdateDiscountDto { Percentage = 10, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), ServiceId = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _discountService.UpdateAsync(999, dto, 1, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenExists()
    {
        // Arrange: знайдена знижка
        var discount = new Discount { Id = 1 };

        _mockDiscountRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(discount);

        // Act
        await _discountService.DeleteAsync(1, 1, CancellationToken.None);

        // Assert: перевірка, що викликано видалення та збереження
        _mockDiscountRepo.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        // Arrange: знижку не знайдено
        _mockDiscountRepo.Setup(r => r.GetByIdAsync(404, It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Discount)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _discountService.DeleteAsync(404, 1, CancellationToken.None));
    }

    [Fact]
    public async Task GetActiveDiscountsAsync_ShouldReturnOnlyActiveDiscounts()
    {
        // Arrange: створення списку знижок (одна активна, одна прострочена)
        var now = DateTime.UtcNow;
        var discounts = new List<Discount>
        {
            new Discount { Id = 1, StartDate = now.AddDays(-1), EndDate = now.AddDays(1) }, // Активна
            new Discount { Id = 2, StartDate = now.AddDays(-10), EndDate = now.AddDays(-5) } // Неактивна
        };
        var dtos = new List<DiscountDto> { new DiscountDto { Id = 1, Percentage = 10 } };

        // Налаштування репозиторію та мапера
        _mockDiscountRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(discounts);
        _mockMapper.Setup(m => m.Map<IEnumerable<DiscountDto>>(It.Is<IEnumerable<Discount>>(d => d.Count() == 1)))
                   .Returns(dtos);

        // Act
        var result = await _discountService.GetActiveDiscountsAsync(CancellationToken.None);

        // Assert: лише одна активна знижка
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }
}
