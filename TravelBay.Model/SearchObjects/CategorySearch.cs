namespace TravelBay.Model.SearchObjects
{
    public class CategorySearchObject : BaseSearchObject
    {
        /// <summary>
        /// Substring to match against the category name (case-insensitive).
        /// </summary>
        public string? Name { get; set; }
    }
}
