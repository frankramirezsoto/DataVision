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
    public class ReportsController : ControllerBase
    {
        private readonly IExternalDataService _externalDataService;
        private readonly ILogService _logService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IExternalDataService externalDataService,
            ILogService logService,
            ILogger<ReportsController> logger)
        {
            _externalDataService = externalDataService;
            _logService = logService;
            _logger = logger;
        }

        [HttpGet("fuel-prices-history")]
        public async Task<ActionResult<ApiResponse<ReportDataResponse>>> GetFuelPricesHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                await LogRequest("GET /api/reports/fuel-prices-history", userId);

                var data = await _externalDataService.GetFuelPricesHistoryAsync();

                return Ok(new ApiResponse<ReportDataResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Datos históricos de precios obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching fuel prices history");
                return StatusCode(500, new ApiResponse<ReportDataResponse>
                {
                    Success = false,
                    Message = "Error al obtener datos históricos de precios",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("current-fuel-prices")]
        public async Task<ActionResult<ApiResponse<ReportDataResponse>>> GetCurrentFuelPrices()
        {
            try
            {
                var userId = GetCurrentUserId();
                await LogRequest("GET /api/reports/current-fuel-prices", userId);

                var data = await _externalDataService.GetCurrentFuelPricesAsync();

                return Ok(new ApiResponse<ReportDataResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Precios actuales obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current fuel prices");
                return StatusCode(500, new ApiResponse<ReportDataResponse>
                {
                    Success = false,
                    Message = "Error al obtener precios actuales",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("fuel-sales")]
        public async Task<ActionResult<ApiResponse<ReportDataResponse>>> GetFuelSales()
        {
            try
            {
                var userId = GetCurrentUserId();
                await LogRequest("GET /api/reports/fuel-sales", userId);

                var data = await _externalDataService.GetFuelSalesDataAsync();

                return Ok(new ApiResponse<ReportDataResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Datos de ventas obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching fuel sales data");
                return StatusCode(500, new ApiResponse<ReportDataResponse>
                {
                    Success = false,
                    Message = "Error al obtener datos de ventas",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("all-reports")]
        public async Task<ActionResult<ApiResponse<Dictionary<string, ReportDataResponse>>>> GetAllReports()
        {
            try
            {
                var userId = GetCurrentUserId();
                await LogRequest("GET /api/reports/all-reports", userId);

                var reports = new Dictionary<string, ReportDataResponse>();

                // Fetch all reports concurrently
                var tasks = new List<Task<ReportDataResponse>>
                {
                    _externalDataService.GetFuelPricesHistoryAsync(),
                    _externalDataService.GetCurrentFuelPricesAsync(),
                    _externalDataService.GetFuelSalesDataAsync()
                };

                var results = await Task.WhenAll(tasks);

                reports.Add("fuelPricesHistory", results[0]);
                reports.Add("currentFuelPrices", results[1]);
                reports.Add("fuelSales", results[2]);

                return Ok(new ApiResponse<Dictionary<string, ReportDataResponse>>
                {
                    Success = true,
                    Data = reports,
                    Message = "Todos los reportes obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all reports");
                return StatusCode(500, new ApiResponse<Dictionary<string, ReportDataResponse>>
                {
                    Success = false,
                    Message = "Error al obtener todos los reportes",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("dashboard-summary")]
        public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetDashboardSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                await LogRequest("GET /api/reports/dashboard-summary", userId);

                // Get current prices for summary
                var currentPrices = await _externalDataService.GetCurrentFuelPricesAsync();
                var salesData = await _externalDataService.GetFuelSalesDataAsync();

                var summary = new DashboardSummaryDto
                {
                    TotalFuelTypes = currentPrices.Data?.Count ?? 0,
                    AveragePrice = currentPrices.Data?.Average(d => d.Value) ?? 0,
                    LastUpdated = DateTime.UtcNow,
                    TopFuelByVolume = salesData.Data?.OrderByDescending(d => d.Value).FirstOrDefault()?.Label ?? "N/A",
                    DataSource = "RECOPE - Refinadora Costarricense de Petróleo"
                };

                return Ok(new ApiResponse<DashboardSummaryDto>
                {
                    Success = true,
                    Data = summary,
                    Message = "Resumen del dashboard obtenido exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard summary");
                return StatusCode(500, new ApiResponse<DashboardSummaryDto>
                {
                    Success = false,
                    Message = "Error al obtener resumen del dashboard",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        private async Task LogRequest(string endpoint, int userId, string? parameters = null)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _logService.LogRequestAsync(userId, endpoint);
        }
    }
}
