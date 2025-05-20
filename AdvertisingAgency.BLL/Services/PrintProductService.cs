namespace AdvertisingAgency.BLL.Services
{
    using AdvertisingAgency.BLL.DTOs;
    using AdvertisingAgency.BLL.Exceptions;
    using AdvertisingAgency.BLL.Interfaces;
    using AdvertisingAgency.DAL;
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class PrintProductService : IPrintProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PrintProductService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrintProductDto>> GetAllAsync(PrintProductFilterDto filter, CancellationToken ct = default)
        {
            var products = await _uow.PrintProducts.GetAllWithIncludesAsync(ct);

            var filteredProducts = products.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                filteredProducts = filteredProducts.Where(s => s.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                filteredProducts = filteredProducts.Where(s => s.Category.Name.Contains(filter.Category, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.MinPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(s => s.BaseCost >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(s => s.BaseCost <= filter.MaxPrice.Value);
            }

            var result = _mapper.Map<IEnumerable<PrintProductDto>>(filteredProducts.ToList());

            return result;
        }

        public async Task<PrintProductDto> GetAsync(int id, CancellationToken ct = default)
        {
            var entity = await _uow.PrintProducts.GetByIdAsync(id, ct) ??
                       throw new EntityNotFoundException(nameof(PrintProduct), id);
            return _mapper.Map<PrintProductDto>(entity);
        }

        public async Task<int> CreateAsync(CreatePrintProductDto dto, int actorId, CancellationToken ct = default)
        {
            await EnsureManagerRights(actorId, ct);
            var entity = _mapper.Map<PrintProduct>(dto);
            var id = await _uow.PrintProducts.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return id;
        }

        public async Task UpdateAsync(int id, UpdatePrintProductDto dto, int actorId, CancellationToken ct = default)
        {
            await EnsureManagerRights(actorId, ct);
            var entity = await _uow.PrintProducts.GetByIdAsync(id, ct) ??
                       throw new EntityNotFoundException(nameof(PrintProduct), id);
            _mapper.Map(dto, entity);
            await _uow.PrintProducts.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, int actorId, CancellationToken ct = default)
        {
            await EnsureManagerRights(actorId, ct);
            await _uow.PrintProducts.DeleteAsync(id, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task EnsureManagerRights(int userId, CancellationToken ct)
        {
            var user = await _uow.Users.GetByIdWithIncludesAsync(userId, ct) ??
                      throw new ForbiddenException("User not found.");
            if (user.Role == null || user.Role.Name is not ("Administrator" or "Manager"))
                throw new ForbiddenException("Only admin or manager can modify print products.");
        }
    }
}