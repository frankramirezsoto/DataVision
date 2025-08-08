using DataVision.Data;
using DataVision.DTOs;
using DataVision.Models;
using DataVision.Data;
using DataVision.DTOs;
using DataVision.Models;
using Microsoft.EntityFrameworkCore;

namespace DataVision.Services
{
    public interface ILogService
    {
        Task LogRequestAsync(int userId, string endpoint);
        Task<ApiResponse<List<LogDto>>> GetUserLogsAsync(int userId);
        Task<ApiResponse<List<LogDto>>> GetAllLogsAsync();
        Task<ApiResponse<Dictionary<string, int>>> GetEndpointStatsAsync();
    }

    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogService> _logger;

        public LogService(ApplicationDbContext context, ILogger<LogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogRequestAsync(int userId, string endpoint)
        {
            try
            {
                var log = new Log
                {
                    UserId = userId,
                    EndpointConsultado = endpoint,
                    FechaConsulta = DateTime.UtcNow
                };

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging request for user {UserId}, endpoint {Endpoint}", userId, endpoint);
            }
        }

        public async Task<ApiResponse<List<LogDto>>> GetUserLogsAsync(int userId)
        {
            try
            {
                var logs = await _context.Logs
                    .Include(l => l.User)
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.FechaConsulta)
                    .Select(l => new LogDto
                    {
                        Id = l.Id,
                        Username = l.User.Username,
                        FechaConsulta = l.FechaConsulta,
                        EndpointConsultado = l.EndpointConsultado
                    })
                    .ToListAsync();

                return new ApiResponse<List<LogDto>>
                {
                    Success = true,
                    Data = logs,
                    Message = $"Se encontraron {logs.Count} registros para el usuario"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user logs for user {UserId}", userId);
                return new ApiResponse<List<LogDto>>
                {
                    Success = false,
                    Message = "Error al obtener los logs del usuario",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<LogDto>>> GetAllLogsAsync()
        {
            try
            {
                var logs = await _context.Logs
                    .Include(l => l.User)
                    .OrderByDescending(l => l.FechaConsulta)
                    .Select(l => new LogDto
                    {
                        Id = l.Id,
                        Username = l.User.Username,
                        FechaConsulta = l.FechaConsulta,
                        EndpointConsultado = l.EndpointConsultado
                    })
                    .ToListAsync();

                return new ApiResponse<List<LogDto>>
                {
                    Success = true,
                    Data = logs,
                    Message = $"Se encontraron {logs.Count} registros totales"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all logs");
                return new ApiResponse<List<LogDto>>
                {
                    Success = false,
                    Message = "Error al obtener todos los logs",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<Dictionary<string, int>>> GetEndpointStatsAsync()
        {
            try
            {
                var stats = await _context.Logs
                    .GroupBy(l => l.EndpointConsultado)
                    .Select(g => new { Endpoint = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToDictionaryAsync(x => x.Endpoint, x => x.Count);

                return new ApiResponse<Dictionary<string, int>>
                {
                    Success = true,
                    Data = stats,
                    Message = "Estadísticas de endpoints obtenidas exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching endpoint stats");
                return new ApiResponse<Dictionary<string, int>>
                {
                    Success = false,
                    Message = "Error al obtener estadísticas de endpoints",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}