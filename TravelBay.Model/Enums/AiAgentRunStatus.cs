namespace TravelBay.Model.Enums
{
    /// <summary>
    /// Queued (API published the message) -> Running (worker picked it up) -> Completed / Failed.
    /// The API only creates Queued runs; every later status is written by the worker.
    /// </summary>
    public enum AiAgentRunStatus
    {
        Queued,
        Running,
        Completed,
        Failed
    }
}
