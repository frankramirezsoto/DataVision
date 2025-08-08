using DataVision.DTOs;
using DataVision.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DataVision.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpGet("my-logs")]
        public async Task<ActionResult<ApiResponse<List<LogDto>>>> GetMyLogs()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _logService.LogRequestAsync(userId, "GET /api/logs/my-logs");

                var result = await _logService.GetUserLogsAsync(userId);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user logs for user {UserId}", GetCurrentUserId());
                return StatusCode(500, new ApiResponse<List<LogDto>>
                {
                    Success = false,
                    Message = "Error al obtener los logs del usuario"
                });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<LogDto>>>> GetAllLogs()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _logService.LogRequestAsync(userId, "GET /api/logs/all");

                var result = await _logService.GetAllLogsAsync();

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all logs");
                return StatusCode(500, new ApiResponse<List<LogDto>>
                {
                    Success = false,
                    Message = "Error al obtener todos los logs"
                });
            }
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, int>>>> GetEndpointStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _logService.LogRequestAsync(userId, "GET /api/logs/stats");

                var result = await _logService.GetEndpointStatsAsync();

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching endpoint statistics");
                return StatusCode(500, new ApiResponse<Dictionary<string, int>>
                {
                    Success = false,
                    Message = "Error al obtener estadísticas de endpoints"
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }
    }
}
