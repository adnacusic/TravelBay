namespace TravelBay.Model.Responses
{
    public class AuditLogResponse
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public int? PerformedByUserId { get; set; }
        public string? PerformedByUserName { get; set; }
        public DateTime PerformedAt { get; set; }
        public string? Details { get; set; }
    }
}
