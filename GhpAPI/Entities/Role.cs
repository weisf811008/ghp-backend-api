namespace GhpAPI.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool Reserved { get; set; }= false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserRole> UserRoles  { get; set; } = new List<UserRole>();
    }
}
