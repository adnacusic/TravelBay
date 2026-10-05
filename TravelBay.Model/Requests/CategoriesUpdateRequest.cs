namespace TravelBay.Model.Requests
{
    public class CategoriesUpdateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? IconName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
