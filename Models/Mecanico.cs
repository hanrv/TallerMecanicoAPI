using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TallerMecanicoAPI.Models
{
    public class Mecanico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Especialidad { get; set; } = string.Empty; // Ej: "Motor", "Frenos", "Electricidad"

        [Column(TypeName = "decimal(10,2)")]
        public decimal TarifaHora { get; set; }

        public bool Activo { get; set; } = true;

        // Propiedad de navegación: Un mecánico atiende muchas órdenes
        public List<OrdenTrabajo> OrdenesTrabajo { get; set; } = new();
    }
}