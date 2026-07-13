namespace GhpAPI.Entities
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public string Category { get; set; } = null!;

        public int SchoolId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
