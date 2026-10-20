using Microsoft.EntityFrameworkCore;
using TallerMecanicoAPI.Models;

namespace TallerMecanicoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Representa la tabla Vehiculos en la BD
        public DbSet<Vehiculo> Vehiculos { get; set; }
    }
}