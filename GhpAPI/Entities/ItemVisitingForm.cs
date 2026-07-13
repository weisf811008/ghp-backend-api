namespace GhpAPI.Entities
{
    public class ItemVisitingForm
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int VisitingFormId { get; set; }

        public InspectionItem? Item { get; set; }
        public VisitingForm? VisitingForm { get; set; }
    }
}
