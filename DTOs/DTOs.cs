using System.ComponentModel.DataAnnotations;

namespace DataVision.DTOs
{
    // Auth DTOs
    public class LoginRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(20)]
        public string Role { get; set; } = "User";
    }

    // Report DTOs
    public class ReportDataResponse
    {
        public string ChartType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<DataPoint> Data { get; set; } = new List<DataPoint>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class DataPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string? Color { get; set; }
        public string? Category { get; set; }
    }

    // Log DTOs
    public class LogDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public string EndpointConsultado { get; set; } = string.Empty;
        public string? Parameters { get; set; }
        public string? IpAddress { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    // Additional DTO for dashboard summary
    public class DashboardSummaryDto
    {
        public int TotalFuelTypes { get; set; }
        public decimal AveragePrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public string TopFuelByVolume { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
    }
}