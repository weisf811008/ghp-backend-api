namespace GhpAPI.Entities
{
    public class Inspection
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Remarks { get; set; }
        public int Version { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ParentId { get; set; }
        public int FormId { get; set; }
        public int SchoolId { get; set; }
        public int InspectedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
