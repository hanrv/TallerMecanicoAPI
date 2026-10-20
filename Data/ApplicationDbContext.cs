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

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Mecanico> Mecanicos { get; set; }
        public DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; }
        public DbSet<Repuesto> Repuestos { get; set; }
    }
}