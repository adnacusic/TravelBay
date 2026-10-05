using TravelBay.Model.Constants;
using TravelBay.Model.Requests;
using TravelBay.Model.Responses;
using TravelBay.Model.SearchObjects;
using TravelBay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers;

/// <summary>
/// Admin manages every user here (list/detail/create/edit/deactivate/reset password); any
/// authenticated user manages only their own profile through the "Me" actions.
/// </summary>
[Authorize]
[Route("[controller]")]
public class UsersController : BaseCRUDController<UserResponse, UserSearch, UserInsertRequest, UserUpdateRequest, IUserService>
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public UsersController(IUserService userService, IAuthenticatedUserAccessor userAccessor) : base(userService)
    {
        _userAccessor = userAccessor;
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<PageResult<UserResponse>> GetAll([FromQuery] UserSearch? search)
    {
        return base.GetAll(search);
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<ActionResult<UserResponse>> GetById(int id)
    {
        return base.GetById(id);
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<ActionResult<UserResponse>> Create([FromBody] UserInsertRequest request)
    {
        return base.Create(request);
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<ActionResult<UserResponse>> Update(int id, [FromBody] UserUpdateRequest request)
    {
        return base.Update(id, request);
    }

    /// <summary>Deactivates the account (IsActive = false) — never a hard delete.</summary>
    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Delete(int id)
    {
        return base.Delete(id);
    }

    [HttpGet("Me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var result = await _service.GetByIdAsync(CurrentUserId());
        return Ok(result);
    }

    /// <summary>Own-profile edit — no password field here; see Me/ChangePassword for that.</summary>
    [HttpPut("Me")]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UserUpdateRequest request)
    {
        var result = await _service.UpdateAsync(CurrentUserId(), request);
        return Ok(result);
    }

    [HttpPut("Me/ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeRequest request)
    {
        await _service.ChangePasswordAsync(CurrentUserId(), request);
        return Ok(new { message = "Lozinka je uspješno promijenjena." });
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{id}/ResetPassword")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] UserPasswordResetRequest request)
    {
        await _service.ResetPasswordAsync(id, request);
        return Ok(new { message = "Lozinka korisnika je uspješno resetovana." });
    }

    private int CurrentUserId() =>
        _userAccessor.GetUserId() ?? throw new InvalidOperationException("User id claim is missing.");
}
