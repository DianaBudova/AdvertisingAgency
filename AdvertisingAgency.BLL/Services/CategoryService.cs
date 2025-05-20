namespace AdvertisingAgency.BLL.Services
{
    using AutoMapper;
    using AdvertisingAgency.BLL.DTOs;
    using AdvertisingAgency.BLL.Exceptions;
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;
    using AdvertisingAgency.BLL.Interfaces;

    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateCategoryDto dto, int userId, CancellationToken ct = default)
        {
            await EnsureManagerRights(userId, ct);

            if (await CategoryExists(dto.Name, ct))
                throw new ValidationException("Category already in use.");

            var category = new Category { Name = dto.Name };
            var id = await _uow.Categories.AddAsync(category, ct);
            await _uow.SaveChangesAsync(ct);
            return id;
        }

        public async Task DeleteAsync(int id, int userId, CancellationToken ct = default)
        {
            await EnsureManagerRights(userId, ct);

            var category = await _uow.Categories.GetByIdAsync(id, ct);
            if (category == null)
                throw new EntityNotFoundException(nameof(Category), id);

            await _uow.Categories.DeleteAsync(id, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
        {
            var orders = await _uow.Categories.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<CategoryDto>>(orders);
        }

        public async Task UpdateAsync(int id, UpdateCategoryDto dto, int userId, CancellationToken ct = default)
        {
            await EnsureManagerRights(userId, ct);

            var category = await _uow.Categories.GetByIdAsync(id, ct) ?? throw new EntityNotFoundException(nameof(Category), id);

            if (await CategoryExists(dto.Name, ct) &&
                !string.Equals(category.Name, dto.Name, System.StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("Another category with the same name already exists.");
            }

            category.Name = dto.Name;
            await _uow.Categories.UpdateAsync(category, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<bool> CategoryExists(string category, CancellationToken ct)
        {
            return (await _uow.Categories.GetAllAsync(ct)).Any(u => u.Name == category);
        }

        private async Task EnsureManagerRights(int userId, CancellationToken ct)
        {
            var user = await _uow.Users.GetByIdWithIncludesAsync(userId, ct) ??
                       throw new ForbiddenException("User not found.");
            if (user.Role == null || user.Role.Name is not ("Administrator" or "Manager"))
                throw new ForbiddenException("Only admin or manager can modify services.");
        }
    }
}