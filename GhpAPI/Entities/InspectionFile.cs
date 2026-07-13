namespace GhpAPI.Entities
{
    public class InspectionFile
    {
        public int Id { get; set; }
        public string Filename { get; set; } = null!;
        public string Originalname { get; set; } = null!;
        public string Encoding { get; set; } = null!;
        public string Mimetype { get; set; } = null!;
        public int InspectionId { get; set; }
        public int ItemId { get; set; }
    }
}
