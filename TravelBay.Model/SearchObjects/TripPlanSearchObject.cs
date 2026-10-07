using TravelBay.Model.Enums;

namespace TravelBay.Model.SearchObjects
{
    public class TripPlanSearchObject : BaseSearchObject
    {
        /// <summary>Admin-only: list another user's plans. Ignored for non-admin callers (forced to their own id).</summary>
        public int? UserId { get; set; }

        public TripPlanStatus? Status { get; set; }

        /// <summary>true = completed or cancelled (history), false = draft or active (current).</summary>
        public bool? IsFinished { get; set; }
    }
}
