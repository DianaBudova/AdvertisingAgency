using Microsoft.EntityFrameworkCore;

namespace AdvertisingAgency.DAL
{
    using AdvertisingAgency.DAL.Entities;

    public class AdvertisingAgencyContext : DbContext
    {
        public AdvertisingAgencyContext(DbContextOptions<AdvertisingAgencyContext> options)
            : base(options) { }

        public DbSet<Service> Services => Set<Service>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Discount> Discounts => Set<Discount>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<PrintProduct> PrintProducts => Set<PrintProduct>();
        public DbSet<QuickOrder> QuickOrders => Set<QuickOrder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdvertisingAgencyContext).Assembly);

            var roles = new[]
            {
                new Role { Id = 1, Name = "Administrator" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "Registered" },
                new Role { Id = 4, Name = "Unregistered" }
            };
            modelBuilder.Entity<Role>().HasData(roles);
        }
    }
}
