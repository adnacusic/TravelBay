using TravelBay.Model.Enums;

namespace TravelBay.Model.SearchObjects;

public class ReviewSearchObject : BaseSearchObject
{
    /// <summary>When the caller is an administrator, restricts results to reviews by this user.</summary>
    public int? UserId { get; set; }

    public int? DestinationId { get; set; }

    public ReviewStatus? Status { get; set; }

    public int? Rating { get; set; }

    /// <summary>Only the caller's own reviews (any status), e.g. to show "your review is waiting for approval".</summary>
    public bool? OnlyMine { get; set; }
}
