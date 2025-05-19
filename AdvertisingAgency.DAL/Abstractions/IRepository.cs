namespace AdvertisingAgency.DAL.Abstractions
{
    using AdvertisingAgency.DAL.Entities;

    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<TEntity?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(TEntity entity, CancellationToken ct = default);
        Task UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
