using Microsoft.AspNetCore.Mvc;
using InclusingLenguage.API.Models;
using InclusingLenguage.API.Services;
using Microsoft.Extensions.Options;

namespace InclusingLenguage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly MongoDBSettings _mongoDBSettings;

        public AuthController(IAuthService authService, IOptions<MongoDBSettings> mongoDBSettings)
        {
            _authService = authService;
            _mongoDBSettings = mongoDBSettings.Value;
        }

        [HttpGet("config")]
        public ActionResult GetConfig()
        {
            return Ok(new
            {
                DatabaseName = _mongoDBSettings.DatabaseName,
                ConnectionStringHint = _mongoDBSettings.ConnectionString.Contains("inclusign") ? "inclusign" : "includesign"
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Email y contraseña son requeridos"
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Email y contraseña son requeridos"
                });
            }

            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
