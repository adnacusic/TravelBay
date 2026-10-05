namespace TravelBay.Model.SearchObjects
{
    public class AuditLogSearchObject : BaseSearchObject
    {
        public string? EntityName { get; set; }
        public int? EntityId { get; set; }
    }
}
