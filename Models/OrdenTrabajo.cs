using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TallerMecanicoAPI.Models
{
    public class OrdenTrabajo
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaApertura { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente"; // "Pendiente", "En Proceso", "Completado"

        [Required]
        [MaxLength(300)]
        public string Diagnostico { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoManoObra { get; set; }

        // Llave Foránea hacia Vehiculo
        [Required]
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        // Llave Foránea hacia Mecanico
        [Required]
        public int MecanicoId { get; set; }
        public Mecanico? Mecanico { get; set; }

        // Propiedad de navegación: Una orden puede usar varios repuestos
        public List<Repuesto> Repuestos { get; set; } = new();
    }
}