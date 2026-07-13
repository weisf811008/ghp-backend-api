namespace GhpAPI.Entities
{
    public class VisitingForm
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Class { get; set; } = null!;

        public string Description { get; set; } = null!;
        public int SchoolId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ItemVisitingForm> ItemVisitingForms { get; set; } = new List<ItemVisitingForm>();
    }
}
