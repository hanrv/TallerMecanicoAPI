using System.ComponentModel.DataAnnotations;

namespace TallerMecanicoAPI.Models
{
    // Esta clase representa la tabla 'Vehiculos' en SQL Server
    public class Vehiculo
    {
        [Key] // Llave primaria autoincremental
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
    }

    // Modelo para peticiones HTTP PATCH (campos opcionales)
    public class VehiculoPatch
    {
        [MaxLength(50)]
        public string? Marca { get; set; }

        [MaxLength(50)]
        public string? Modelo { get; set; }

        public int? Anio { get; set; }

        [MaxLength(250)]
        public string? FallaReportada { get; set; }
    }
}