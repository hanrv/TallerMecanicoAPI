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

        // Inyección del DbContext en el constructor
        public VehiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/vehiculos (Listar todos los vehículos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> ObtenerTodos()
        {
            var vehiculos = await _context.Vehiculos
                .OrderByDescending(v => v.FechaIngreso)
                .ToListAsync();

            return Ok(vehiculos);
        }

        // 2. GET: api/vehiculos/{placa} (Buscar vehículo específico por placa)
        [HttpGet("{placa}")]
        public async Task<ActionResult<Vehiculo>> ObtenerPorPlaca(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound(new { mensaje = $"No se encontró ningún vehículo con placa {placa}." });
            }

            return Ok(vehiculo);
        }

        // 3. GET: api/vehiculos/buscar?marca=Toyota&anioMinimo=2020 (Filtros dinámicos con LINQ)
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> Buscar(
            [FromQuery] string? marca,
            [FromQuery] int? anioMinimo)
        {
            var query = _context.Vehiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca));
            }

            if (anioMinimo.HasValue)
            {
                query = query.Where(v => v.Anio >= anioMinimo.Value);
            }

            var resultados = await query.ToListAsync();
            return Ok(resultados);
        }

        // 4. POST: api/vehiculos (Registrar un nuevo vehículo)
        [HttpPost]
        public async Task<ActionResult<Vehiculo>> CrearVehiculo([FromBody] Vehiculo nuevoVehiculo)
        {
            // Validar que no se dupliquen placas
            bool yaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa.ToLower() == nuevoVehiculo.Placa.ToLower());

            if (yaExiste)
            {
                return BadRequest(new { mensaje = $"El vehículo con placa {nuevoVehiculo.Placa} ya está registrado." });
            }

            nuevoVehiculo.FechaIngreso = DateTime.Now;

            _context.Vehiculos.Add(nuevoVehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerPorPlaca),
                new { placa = nuevoVehiculo.Placa },
                nuevoVehiculo);
        }

        // 5. PUT: api/vehiculos/{placa} (Actualización completa)
        [HttpPut("{placa}")]
        public async Task<IActionResult> Actualizar(string placa, [FromBody] Vehiculo vehiculoActualizado)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound(new { mensaje = $"No se encontró ningún vehículo con placa {placa}." });
            }

            vehiculo.Marca = vehiculoActualizado.Marca;
            vehiculo.Modelo = vehiculoActualizado.Modelo;
            vehiculo.Anio = vehiculoActualizado.Anio;
            vehiculo.FallaReportada = vehiculoActualizado.FallaReportada;

            await _context.SaveChangesAsync();
            return Ok(vehiculo);
        }

        // 6. PATCH: api/vehiculos/{placa} (Actualización parcial)
        [HttpPatch("{placa}")]
        public async Task<IActionResult> ActualizarParcial(string placa, [FromBody] VehiculoPatch cambios)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound(new { mensaje = $"No se encontró ningún vehículo con placa {placa}." });
            }

            if (!string.IsNullOrWhiteSpace(cambios.Marca))
                vehiculo.Marca = cambios.Marca;

            if (!string.IsNullOrWhiteSpace(cambios.Modelo))
                vehiculo.Modelo = cambios.Modelo;

            if (cambios.Anio.HasValue)
                vehiculo.Anio = cambios.Anio.Value;

            if (!string.IsNullOrWhiteSpace(cambios.FallaReportada))
                vehiculo.FallaReportada = cambios.FallaReportada;

            await _context.SaveChangesAsync();
            return Ok(vehiculo);
        }

        // 7. DELETE: api/vehiculos/{placa} (Eliminar vehículo)
        [HttpDelete("{placa}")]
        public async Task<IActionResult> Eliminar(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound(new { mensaje = $"No se encontró ningún vehículo con placa {placa}." });
            }

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}