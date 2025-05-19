namespace AdvertisingAgency.DAL.Repositories
{
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;
    using Microsoft.EntityFrameworkCore;

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly AdvertisingAgencyContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(AdvertisingAgencyContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            var navigationProperties = _dbSet.EntityType.GetNavigations();

            foreach (var navProp in navigationProperties)
            {
                query = query.Include(navProp.Name);
            }

            var result = await query.AsNoTracking().ToListAsync(ct);

            return result;
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _dbSet.FindAsync(new object?[] { id }, ct);

        public async Task<TEntity?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();

            var navigationProperties = _context.Model
                .FindEntityType(typeof(TEntity))?
                .GetNavigations()
                .Select(n => n.Name);

            if (navigationProperties != null)
            {
                foreach (var nav in navigationProperties)
                {
                    query = query.Include(nav);
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);
        }

        public async Task<int> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity.Id;
        }

        public Task UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetByIdAsync(id, ct);
            if (entity is null) return;
            _dbSet.Remove(entity);
        }
    }
}
