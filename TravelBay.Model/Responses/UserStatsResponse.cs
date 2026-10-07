namespace TravelBay.Model.Responses
{
    public class UserStatsResponse
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int Administrators { get; set; }

        /// <summary>Accounts created in the last <see cref="NewUsersPeriodDays"/> days.</summary>
        public int NewUsers { get; set; }
        public int NewUsersPeriodDays { get; set; }
    }

    /// <summary>What one user has done in the app (admin user details).</summary>
    public class UserActivityResponse
    {
        public int ReviewCount { get; set; }
        public int PendingReviewCount { get; set; }
        public int ApprovedReviewCount { get; set; }
        public int RejectedReviewCount { get; set; }
        public int TripPlanCount { get; set; }
        public int CompletedTripPlanCount { get; set; }
        public int CollectionCount { get; set; }
        public int SavedDestinationCount { get; set; }
        public int ViewCount { get; set; }
    }
}
