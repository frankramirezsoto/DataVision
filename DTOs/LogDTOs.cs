using System.ComponentModel.DataAnnotations;

namespace DataVision.DTOs
{
    // Log DTOs
    public class LogDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public string EndpointConsultado { get; set; } = string.Empty;
    }
}