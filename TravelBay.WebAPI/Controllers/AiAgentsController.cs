using TravelBay.Model.Constants;
using TravelBay.Model.Enums;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

/// <summary>
/// Desktop "AI Agenti" screen. Starting an agent only publishes a RabbitMQ message — the work is
/// done by the separate worker container, which reports progress into the run read here.
/// </summary>
[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]")]
public class AiAgentsController : ControllerBase
{
    private readonly IAiAgentService _aiAgentService;

    public AiAgentsController(IAiAgentService aiAgentService)
    {
        _aiAgentService = aiAgentService;
    }

    [HttpGet("Status")]
    public async Task<ActionResult<AiAgentStatusResponse>> GetStatus() => Ok(await _aiAgentService.GetStatusAsync());

    [HttpPost("Keywords/Run")]
    public async Task<ActionResult<AiAgentRunResponse>> RunKeywords() =>
        Ok(await _aiAgentService.StartAsync(AiAgentType.Keywords));

    [HttpPost("Images/Run")]
    public async Task<ActionResult<AiAgentRunResponse>> RunImages() =>
        Ok(await _aiAgentService.StartAsync(AiAgentType.Images));

    [HttpGet("Runs")]
    public async Task<ActionResult<PageResult<AiAgentRunResponse>>> GetRuns([FromQuery] AiAgentRunSearchObject? search) =>
        Ok(await _aiAgentService.GetRunsAsync(search));

    [HttpGet("Runs/{id}")]
    public async Task<ActionResult<AiAgentRunResponse>> GetRun(int id) => Ok(await _aiAgentService.GetRunAsync(id));
}
