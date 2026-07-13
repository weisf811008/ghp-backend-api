namespace GhpAPI.Entities
{
    public class Form
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Remarks { get; set; }
        public int SchoolId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
