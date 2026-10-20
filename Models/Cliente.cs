using System.ComponentModel.DataAnnotations;

namespace TallerMecanicoAPI.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        // Propiedad de navegación: Un cliente tiene muchos vehículos
        public List<Vehiculo> Vehiculos { get; set; } = new();
    }
}