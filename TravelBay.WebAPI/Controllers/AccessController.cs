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
        private readonly IPasswordResetService _passwordResetService;

        public AccessController(IAccessManager accessManager, IUserService userService, IPasswordResetService passwordResetService)
        {
            _accessManager = accessManager;
            _userService = userService;
            _passwordResetService = passwordResetService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            var result = await _accessManager.LoginAsync(request);
            return Ok(result);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            await _userService.RegisterAsync(request);
            return Ok(new { message = "Račun je uspješno kreiran." });
        }

        /// <summary>Emails a 6-digit code (through the worker). The answer is the same whether or not the email exists.</summary>
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _passwordResetService.RequestCodeAsync(request);
            return Ok(new { message = "Ako postoji račun s tim emailom, poslali smo mu kod za promjenu lozinke." });
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _passwordResetService.ResetPasswordAsync(request);
            return Ok(new { message = "Lozinka je promijenjena. Prijavite se novom lozinkom." });
        }
    }
}
