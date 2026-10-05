namespace TravelBay.Model.SearchObjects
{
    public class CollectionSearchObject : BaseSearchObject
    {
        /// <summary>Admin-only: list another user's collections. Ignored for non-admin callers (forced to their own id).</summary>
        public int? UserId { get; set; }
    }
}
