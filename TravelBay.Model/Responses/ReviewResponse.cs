using TravelBay.Model.Enums;

namespace TravelBay.Model.Responses;

public class ReviewResponse
{
    public int Id { get; set; }
    public int DestinationId { get; set; }
    public int UserId { get; set; }
    public string ReviewerDisplayName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public ReviewStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ModeratedByUserId { get; set; }
    public DateTime? ModeratedAt { get; set; }
    public string? ModerationReason { get; set; }
}
