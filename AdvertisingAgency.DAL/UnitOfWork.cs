namespace AdvertisingAgency.DAL
{
    using AdvertisingAgency.DAL.Abstractions;
    using AdvertisingAgency.DAL.Entities;
    using AdvertisingAgency.DAL.Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdvertisingAgencyContext _context;

        public UnitOfWork(AdvertisingAgencyContext context)
        {
            _context = context;
            Services = new Repository<Service>(_context);
            Categories = new Repository<Category>(_context);
            Discounts = new Repository<Discount>(_context);
            Orders = new Repository<Order>(_context);
            OrderItems = new Repository<OrderItem>(_context);
            Users = new Repository<User>(_context);
            Roles = new Repository<Role>(_context);
            PrintProducts = new Repository<PrintProduct>(_context);
            QuickOrders = new Repository<QuickOrder>(_context);
        }

        public IRepository<Service> Services { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<Discount> Discounts { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderItem> OrderItems { get; }
        public IRepository<User> Users { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<PrintProduct> PrintProducts { get; }
        public IRepository<QuickOrder> QuickOrders { get; }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            await _context.SaveChangesAsync(ct);

        public void Dispose() => _context.Dispose();
    }
}