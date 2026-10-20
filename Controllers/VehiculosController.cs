using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanicoAPI.Data;
using TallerMecanicoAPI.Models;

namespace TallerMecanicoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/vehiculos (Trae los vehículos incluyendo los datos de su Cliente)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> ObtenerTodos()
        {
            var vehiculos = await _context.Vehiculos
                .Include(v => v.Cliente) // JOIN con tabla Clientes
                .OrderByDescending(v => v.FechaIngreso)
                .ToListAsync();

            return Ok(vehiculos);
        }

        // 2. GET: api/vehiculos/{placa} (Trae Vehículo + Cliente + Historial de Órdenes + Mecánico)
        [HttpGet("{placa}")]
        public async Task<ActionResult<Vehiculo>> ObtenerPorPlaca(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.OrdenesTrabajo)
                    .ThenInclude(o => o.Mecanico)
                .Include(v => v.OrdenesTrabajo)
                    .ThenInclude(o => o.Repuestos)
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
                return NotFound(new { mensaje = $"No se encontró el vehículo con placa {placa}." });

            return Ok(vehiculo);
        }

        // 3. FILTRO AVANZADO COMBINANDO TABLAS: Vehiculos + Clientes
        // GET: api/vehiculos/buscar?marca=Toyota&anioMinimo=2020&dniCliente=71234567&nombreCliente=Juan
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarAvanzado(
            [FromQuery] string? marca,
            [FromQuery] int? anioMinimo,
            [FromQuery] string? dniCliente,
            [FromQuery] string? nombreCliente)
        {
            var query = _context.Vehiculos
                .Include(v => v.Cliente)
                .Include(v => v.OrdenesTrabajo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(marca))
                query = query.Where(v => v.Marca.Contains(marca));

            if (anioMinimo.HasValue)
                query = query.Where(v => v.Anio >= anioMinimo.Value);

            // Filtro cruzado hacia la tabla Cliente
            if (!string.IsNullOrWhiteSpace(dniCliente))
                query = query.Where(v => v.Cliente != null && v.Cliente.Dni == dniCliente);

            if (!string.IsNullOrWhiteSpace(nombreCliente))
                query = query.Where(v => v.Cliente != null && v.Cliente.NombreCompleto.Contains(nombreCliente));

            // Proyección combinada de varias tablas
            var resultados = await query.Select(v => new
            {
                v.Id,
                v.Placa,
                v.Marca,
                v.Modelo,
                v.Anio,
                v.FallaReportada,
                Propietario = v.Cliente != null ? v.Cliente.NombreCompleto : "Sin propietario asignado",
                DniPropietario = v.Cliente != null ? v.Cliente.Dni : "-",
                TelefonoPropietario = v.Cliente != null ? v.Cliente.Telefono : "-",
                TotalOrdenesRegistradas = v.OrdenesTrabajo.Count
            }).ToListAsync();

            return Ok(resultados);
        }

        // 4. POST: api/vehiculos
        [HttpPost]
        public async Task<ActionResult<Vehiculo>> CrearVehiculo([FromBody] Vehiculo nuevoVehiculo)
        {
            bool yaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa.ToLower() == nuevoVehiculo.Placa.ToLower());

            if (yaExiste)
                return BadRequest(new { mensaje = $"La placa {nuevoVehiculo.Placa} ya está registrada." });

            nuevoVehiculo.FechaIngreso = DateTime.Now;
            _context.Vehiculos.Add(nuevoVehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorPlaca), new { placa = nuevoVehiculo.Placa }, nuevoVehiculo);
        }

        // 5. PUT: api/vehiculos/{placa}
        [HttpPut("{placa}")]
        public async Task<IActionResult> Actualizar(string placa, [FromBody] Vehiculo vehiculoActualizado)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
                return NotFound(new { mensaje = $"No se encontró el vehículo con placa {placa}." });

            vehiculo.Marca = vehiculoActualizado.Marca;
            vehiculo.Modelo = vehiculoActualizado.Modelo;
            vehiculo.Anio = vehiculoActualizado.Anio;
            vehiculo.FallaReportada = vehiculoActualizado.FallaReportada;
            vehiculo.ClienteId = vehiculoActualizado.ClienteId;

            await _context.SaveChangesAsync();
            return Ok(vehiculo);
        }

        // 6. PATCH: api/vehiculos/{placa}
        [HttpPatch("{placa}")]
        public async Task<IActionResult> ActualizarParcial(string placa, [FromBody] VehiculoPatch cambios)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
                return NotFound(new { mensaje = $"No se encontró el vehículo con placa {placa}." });

            if (!string.IsNullOrWhiteSpace(cambios.Marca)) vehiculo.Marca = cambios.Marca;
            if (!string.IsNullOrWhiteSpace(cambios.Modelo)) vehiculo.Modelo = cambios.Modelo;
            if (cambios.Anio.HasValue) vehiculo.Anio = cambios.Anio.Value;
            if (!string.IsNullOrWhiteSpace(cambios.FallaReportada)) vehiculo.FallaReportada = cambios.FallaReportada;
            if (cambios.ClienteId.HasValue) vehiculo.ClienteId = cambios.ClienteId.Value;

            await _context.SaveChangesAsync();
            return Ok(vehiculo);
        }

        // 7. DELETE: api/vehiculos/{placa}
        [HttpDelete("{placa}")]
        public async Task<IActionResult> Eliminar(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
                return NotFound(new { mensaje = $"No se encontró el vehículo con placa {placa}." });

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}