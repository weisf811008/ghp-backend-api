namespace GhpAPI.Entities
{
    public class InspectionItem
    {
        public int Id { get; set; }
        public string No { get; set; } = null!;
        public string Item { get; set; } = null!;
        public string? Period { get; set; }
        public string? Area { get; set; }
        public bool NeedCheckValue { get; set; } = true;
        public bool NeedDaily { get; set; }
        public int CategoryId { get; set; }
        public int SchoolId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public CategoryItem? CategoryItem { get; set; }
        public ICollection<ItemRegulation> ItemRegulations { get; set; } = new List<ItemRegulation>();
        public ICollection<ItemVisitingForm> ItemVisitingForms { get; set; } = new List<ItemVisitingForm>();
    }
}
