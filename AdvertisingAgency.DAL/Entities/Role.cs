namespace AdvertisingAgency.DAL.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<User>? Users { get; set; }
    }
}
