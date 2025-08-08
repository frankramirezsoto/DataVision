using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataVision.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(255)]
        public string EndpointConsultado { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}