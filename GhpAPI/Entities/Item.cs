namespace GhpAPI.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Class { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int schoolId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
