using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanicoAPI.Data;
using TallerMecanicoAPI.Models;

namespace TallerMecanicoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TallerConsultasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TallerConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // ENDPOINTS PARA REGISTRAR LAS ENTIDADES (POST)
        // ============================================================

        [HttpPost("clientes")]
        public async Task<IActionResult> CrearCliente([FromBody] Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpPost("mecanicos")]
        public async Task<IActionResult> CrearMecanico([FromBody] Mecanico mecanico)
        {
            _context.Mecanicos.Add(mecanico);
            await _context.SaveChangesAsync();
            return Ok(mecanico);
        }

        [HttpPost("ordenes")]
        public async Task<IActionResult> CrearOrden([FromBody] OrdenTrabajo orden)
        {
            _context.OrdenesTrabajo.Add(orden);
            await _context.SaveChangesAsync();
            return Ok(orden);
        }

        [HttpPost("repuestos")]
        public async Task<IActionResult> AgregarRepuesto([FromBody] Repuesto repuesto)
        {
            _context.Repuestos.Add(repuesto);
            await _context.SaveChangesAsync();
            return Ok(repuesto);
        }

        // ============================================================
        // CONSULTAS Y FILTROS AVANZADOS COMBINANDO LAS 5 TABLAS
        // ============================================================

        // 1. FILTRO MULTITABLA: Cruza OrdenTrabajo + Vehiculo + Cliente + Mecanico + Repuestos
        // GET: api/TallerConsultas/ordenes/filtrar?estado=En Proceso&especialidad=Frenos&marcaVehiculo=Toyota
        [HttpGet("ordenes/filtrar")]
        public async Task<IActionResult> FiltrarOrdenesAvanzado(
            [FromQuery] string? estado,
            [FromQuery] string? especialidadMecanico,
            [FromQuery] string? marcaVehiculo,
            [FromQuery] decimal? costoMinimoTotal)
        {
            var query = _context.OrdenesTrabajo
                .Include(o => o.Vehiculo)
                    .ThenInclude(v => v!.Cliente)
                .Include(o => o.Mecanico)
                .Include(o => o.Repuestos)
                .AsQueryable();

            // Filtro en tabla OrdenTrabajo
            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(o => o.Estado == estado);

            // Filtro en tabla Mecanico
            if (!string.IsNullOrWhiteSpace(especialidadMecanico))
                query = query.Where(o => o.Mecanico != null && o.Mecanico.Especialidad.Contains(especialidadMecanico));

            // Filtro en tabla Vehiculo
            if (!string.IsNullOrWhiteSpace(marcaVehiculo))
                query = query.Where(o => o.Vehiculo != null && o.Vehiculo.Marca.Contains(marcaVehiculo));

            // Filtro calculado combinando OrdenTrabajo (Mano de Obra) + Suma de tabla Repuestos
            if (costoMinimoTotal.HasValue)
            {
                query = query.Where(o =>
                    (o.CostoManoObra + o.Repuestos.Sum(r => r.Cantidad * r.PrecioUnitario)) >= costoMinimoTotal.Value);
            }

            // Resultado combinado (JOIN de las 5 tablas en un JSON limpio)
            var resultado = await query.Select(o => new
            {
                OrdenId = o.Id,
                o.FechaApertura,
                o.Estado,
                o.Diagnostico,
                VehiculoPlaca = o.Vehiculo != null ? o.Vehiculo.Placa : "",
                VehiculoMarcaModelo = o.Vehiculo != null ? $"{o.Vehiculo.Marca} {o.Vehiculo.Modelo}" : "",
                ClienteNombre = (o.Vehiculo != null && o.Vehiculo.Cliente != null) ? o.Vehiculo.Cliente.NombreCompleto : "Sin cliente",
                MecanicoAsignado = o.Mecanico != null ? o.Mecanico.Nombre : "",
                Especialidad = o.Mecanico != null ? o.Mecanico.Especialidad : "",
                CostoManoObra = o.CostoManoObra,
                TotalRepuestos = o.Repuestos.Sum(r => r.Cantidad * r.PrecioUnitario),
                CostoTotalLiquidacion = o.CostoManoObra + o.Repuestos.Sum(r => r.Cantidad * r.PrecioUnitario),
                DetalleRepuestos = o.Repuestos.Select(r => new
                {
                    r.Nombre,
                    r.Cantidad,
                    r.PrecioUnitario,
                    Subtotal = r.Cantidad * r.PrecioUnitario
                })
            }).ToListAsync();

            return Ok(resultado);
        }

        // 2. REPORTE POR MECÁNICO: Productividad combinando Mecanicos + OrdenesTrabajo + Vehiculos
        // GET: api/TallerConsultas/mecanicos/productividad?soloActivos=true
        [HttpGet("mecanicos/productividad")]
        public async Task<IActionResult> ObtenerProductividadMecanicos([FromQuery] bool soloActivos = true)
        {
            var query = _context.Mecanicos.AsQueryable();

            if (soloActivos)
                query = query.Where(m => m.Activo);

            var reporte = await query.Select(m => new
            {
                m.Id,
                m.Nombre,
                m.Especialidad,
                m.TarifaHora,
                TotalOrdenesAtendidas = m.OrdenesTrabajo.Count,
                OrdenesCompletadas = m.OrdenesTrabajo.Count(o => o.Estado == "Completado"),
                TotalRecaudadoManoObra = m.OrdenesTrabajo.Sum(o => o.CostoManoObra),
                PlacasAtendidas = m.OrdenesTrabajo
                    .Where(o => o.Vehiculo != null)
                    .Select(o => o.Vehiculo!.Placa)
                    .Distinct()
            }).ToListAsync();

            return Ok(reporte);
        }

        // 3. ESTADO DE CUENTA DE CLIENTE: Cruza Cliente -> Vehiculos -> OrdenesTrabajo -> Repuestos
        // GET: api/TallerConsultas/clientes/{dni}/estado-cuenta
        [HttpGet("clientes/{dni}/estado-cuenta")]
        public async Task<IActionResult> ObtenerEstadoCuentaCliente(string dni)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Vehiculos)
                    .ThenInclude(v => v.OrdenesTrabajo)
                        .ThenInclude(o => o.Repuestos)
                .FirstOrDefaultAsync(c => c.Dni == dni);

            if (cliente == null)
                return NotFound(new { mensaje = $"No existe un cliente con DNI {dni}." });

            var resumen = new
            {
                cliente.NombreCompleto,
                cliente.Dni,
                cliente.Telefono,
                CantidadVehiculos = cliente.Vehiculos.Count,
                GastoTotalHistorico = cliente.Vehiculos
                    .SelectMany(v => v.OrdenesTrabajo)
                    .Sum(o => o.CostoManoObra + o.Repuestos.Sum(r => r.Cantidad * r.PrecioUnitario)),
                Vehiculos = cliente.Vehiculos.Select(v => new
                {
                    v.Placa,
                    v.Marca,
                    v.Modelo,
                    Ordenes = v.OrdenesTrabajo.Select(o => new
                    {
                        o.Id,
                        o.FechaApertura,
                        o.Estado,
                        o.Diagnostico,
                        TotalOrden = o.CostoManoObra + o.Repuestos.Sum(r => r.Cantidad * r.PrecioUnitario)
                    })
                })
            };

            return Ok(resumen);
        }
    }
}