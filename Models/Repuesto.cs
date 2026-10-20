using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TallerMecanicoAPI.Models
{
    public class Repuesto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty; // Ej: "Pastillas de freno Bosch"

        [Required]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        // Llave Foránea hacia OrdenTrabajo
        [Required]
        public int OrdenTrabajoId { get; set; }
        public OrdenTrabajo? OrdenTrabajo { get; set; }
    }
}