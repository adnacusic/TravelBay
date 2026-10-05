using TravelBay.Model.Constants;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

/// <summary>Read-only audit trail — entries are written internally by the state machines, never via this API.</summary>
[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<AuditLogResponse>>> GetAll([FromQuery] AuditLogSearchObject? search)
    {
        var result = await _auditLogService.GetAllAsync(search);
        return Ok(result);
    }
}
