namespace TravelBay.Model.Requests
{
    /// <summary>Replaces the caller's entire preferred-category set.</summary>
    public class SetUserPreferencesRequest
    {
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
