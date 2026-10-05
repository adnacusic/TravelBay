using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserPreferencesController : ControllerBase
{
    private readonly IUserPreferenceService _userPreferenceService;

    public UserPreferencesController(IUserPreferenceService userPreferenceService)
    {
        _userPreferenceService = userPreferenceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserPreferenceResponse>>> GetAll() => Ok(await _userPreferenceService.GetAllAsync());

    [HttpPut]
    public async Task<ActionResult<List<UserPreferenceResponse>>> Set([FromBody] SetUserPreferencesRequest request) =>
        Ok(await _userPreferenceService.SetAsync(request));
}
