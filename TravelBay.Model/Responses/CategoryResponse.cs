namespace TravelBay.Model.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? IconName { get; set; }
        public bool IsActive { get; set; }

        /// <summary>Filled in lists only: destinations (not deleted) in this category.</summary>
        public int DestinationCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
