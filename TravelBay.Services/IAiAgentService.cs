using TravelBay.Model.Enums;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;

namespace TravelBay.Services;

/// <summary>Admin side of the AI agents: start a run (RabbitMQ message) and follow its progress.</summary>
public interface IAiAgentService
{
    Task<AiAgentStatusResponse> GetStatusAsync();

    /// <summary>Creates a Queued run and publishes it; refuses while the same agent is still queued or running.</summary>
    Task<AiAgentRunResponse> StartAsync(AiAgentType agentType);

    Task<PageResult<AiAgentRunResponse>> GetRunsAsync(AiAgentRunSearchObject? search);

    Task<AiAgentRunResponse> GetRunAsync(int id);
}
