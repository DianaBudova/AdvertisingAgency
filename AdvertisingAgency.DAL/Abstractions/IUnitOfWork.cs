namespace AdvertisingAgency.DAL.Abstractions
{
    using AdvertisingAgency.DAL.Entities;
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Service> Services { get; }
        IRepository<Category> Categories { get; }
        IRepository<Discount> Discounts { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        IRepository<PrintProduct> PrintProducts { get; }
        IRepository<QuickOrder> QuickOrders { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
