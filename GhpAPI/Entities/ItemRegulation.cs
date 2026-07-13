namespace GhpAPI.Entities
{
    public class ItemRegulation
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int RegulationId { get; set; }

        public InspectionItem? Item { get; set; }
        public Regulation? Regulation { get; set; }

    }
}
