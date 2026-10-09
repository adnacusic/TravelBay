using Azure;
using TravelBay.Model.Access;
using TravelBay.Model.Requests;
using TravelBay.Services;
using TravelBay.WebAPI.Services.AccessManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelBay.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccessController : Controller
    {
        private readonly IAccessManager _accessManager;
        private readonly IUserService _userService;

        public AccessController(IAccessManager accessManager, IUserService userService)
        {
            _accessManager = accessManager;
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            var result = await _accessManager.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("LoginWithRefreshToken")]
        public async Task<ActionResult> LoginWithRefreshToken([FromBody] RefreshAccessTokenRequest request)
        {
            var result = await _accessManager.LoginWithRefreshTokenAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _accessManager.LogoutAsync();
            return Ok(new { message = "Uspješno ste se odjavili." });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            await _userService.RegisterAsync(request);
            return Ok(new { message = "Račun je uspješno kreiran." });
        }
    }
}
