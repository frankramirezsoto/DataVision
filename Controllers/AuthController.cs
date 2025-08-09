using DataVision.DTOs;
using DataVision.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataVision.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = "Datos de entrada inválidos"
                });
            }

            try
            {
                var result = await _authService.AuthenticateAsync(request);

                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Datos de entrada inválidos",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            try
            {
                var result = await _authService.CreateUserAsync(request);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}
