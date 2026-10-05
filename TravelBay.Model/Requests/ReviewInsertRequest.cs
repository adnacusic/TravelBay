namespace TravelBay.Model.Requests;

public class ReviewInsertRequest
{
    public int DestinationId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
