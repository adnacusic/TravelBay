namespace TravelBay.Model.Requests
{
    public class CategoriesInsertRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? IconName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
