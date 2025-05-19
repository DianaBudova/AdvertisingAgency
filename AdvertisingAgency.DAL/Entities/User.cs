namespace AdvertisingAgency.DAL.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public ICollection<Order>? Orders { get; set; }
    }
}
