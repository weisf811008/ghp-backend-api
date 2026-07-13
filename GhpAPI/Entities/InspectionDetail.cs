namespace GhpAPI.Entities
{
    public class InspectionDetail
    {
        public int Id { get; set; }
        public int InspectionId { get; set; }
        public int ItemId { get; set; }
        public string Status { get; set; } = null!;
        public string? Remarks { get; set; }
        public string? CheckValue { get; set; }
    }
}
