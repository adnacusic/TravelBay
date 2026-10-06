namespace TravelBay.Model.SearchObjects
{
    /// <summary>Optional period (whole days, inclusive) for the user activity report.</summary>
    public class UserActivityReportFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class PopularDestinationsReportFilter
    {
        /// <summary>How many destinations to list; defaults to 10, at most 50.</summary>
        public int? Top { get; set; }
    }
}
