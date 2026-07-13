namespace GhpAPI.Entities
{
    public class History
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Type { get; set; }
        public string? Controller { get; set; }
        public string? InstanceKey { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public int? SchoolId { get; set; }
    }
}
