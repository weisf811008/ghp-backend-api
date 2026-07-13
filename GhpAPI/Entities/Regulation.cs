namespace GhpAPI.Entities
{
    public class Regulation
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Class { get; set; } = null!;

        public string Description { get; set; } = null!;
        public int SchoolId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ItemRegulation> ItemRegulations { get; set; } = new List<ItemRegulation>();
    }
}
