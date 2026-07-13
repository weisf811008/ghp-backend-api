namespace GhpAPI.Entities
{
    public class School
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Url { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
