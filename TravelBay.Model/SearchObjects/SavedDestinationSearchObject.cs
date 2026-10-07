namespace TravelBay.Model.SearchObjects
{
    public class SavedDestinationSearchObject : BaseSearchObject
    {
        /// <summary>Checks whether one destination is saved (e.g. on its details screen).</summary>
        public int? DestinationId { get; set; }
    }
}
