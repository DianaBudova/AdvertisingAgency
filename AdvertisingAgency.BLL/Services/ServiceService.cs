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

    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ServiceService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceDto>> GetAllAsync(ServiceFilterDto filter, CancellationToken ct = default)
        {
            var services = await _uow.Services.GetAllWithIncludesAsync(ct);

            var filteredServices = services.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                filteredServices = filteredServices.Where(s => s.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                filteredServices = filteredServices.Where(s => s.Category.Name.Contains(filter.Category, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.MinPrice.HasValue)
            {
                filteredServices = filteredServices.Where(s => s.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                filteredServices = filteredServices.Where(s => s.Price <= filter.MaxPrice.Value);
            }

            var result = _mapper.Map<IEnumerable<ServiceDto>>(filteredServices.ToList());

            return result;
        }

        public async Task<ServiceDto> GetAsync(int id, CancellationToken ct = default)
        {
            var entity = await _uow.Services.GetByIdAsync(id, ct) ??
                       throw new EntityNotFoundException(nameof(Service), id);
            return _mapper.Map<ServiceDto>(entity);
        }

        public async Task<int> CreateAsync(CreateServiceDto dto, int actorId, CancellationToken ct = default)
        {
            await EnsureManagerRights(actorId, ct);
            var entity = _mapper.Map<Service>(dto);
            var id = await _uow.Services.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return id;
        }

        public async Task UpdateAsync(int id, UpdateServiceDto dto, int actorId, CancellationToken ct = default)
        {
            await EnsureManagerRights(actorId, ct);
            var entity = await _uow.Services.GetByIdAsync(id, ct) ??
                       throw new EntityNotFoundException(nameof(Service), id);
            _mapper.Map(dto, entity);
            await _uow.Services.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, int actorId, CancellationToken ct = default)
        {
            await EnsureManagerRights(actorId, ct);
            await _uow.Services.DeleteAsync(id, ct);
            await _uow.SaveChangesAsync(ct);
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