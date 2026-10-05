namespace TravelBay.Model.Constants
{
    /// <summary>Paging limits applied to every list endpoint, so no request can retrieve a whole table.</summary>
    public static class PagingDefaults
    {
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
    }
}
