namespace GhpAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool NeedToChangePass { get; set; } = false;
        public bool Reserved { get; set; } = false;
        public int SchoolId { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public School? School { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
