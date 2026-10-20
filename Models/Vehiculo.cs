using System.ComponentModel.DataAnnotations;

namespace TallerMecanicoAPI.Models
{
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria")]
        [MaxLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es obligatoria")]
        [MaxLength(50)]
        public string Marca { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public int Anio { get; set; }

        [MaxLength(250)]
        public string FallaReportada { get; set; } = string.Empty;

        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        // Llave Foránea opcional o requerida hacia Cliente
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Propiedad de navegación: Un vehículo puede tener muchas órdenes de trabajo
        public List<OrdenTrabajo> OrdenesTrabajo { get; set; } = new();
    }

    public class VehiculoPatch
    {
        [MaxLength(50)]
        public string? Marca { get; set; }

        [MaxLength(50)]
        public string? Modelo { get; set; }

        public int? Anio { get; set; }

        [MaxLength(250)]
        public string? FallaReportada { get; set; }

        public int? ClienteId { get; set; }
    }
}