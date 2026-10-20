using Microsoft.AspNetCore.Mvc;
using TallerMecanicoApi.Models;

namespace TallerMecanicoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // ruta base: api/vehiculos
    public class VehiculosController : ControllerBase
    {
        // La lista es static porque el controller se crea de nuevo en cada
        // peticion; asi los datos se conservan mientras la API este encendida.
        private static List<Vehiculo> vehiculos = new List<Vehiculo>
        {
            new Vehiculo { Placa = "ABC-123", Marca = "Toyota",  Anio = 2018 },
            new Vehiculo { Placa = "XYZ-789", Marca = "Hyundai", Anio = 2021 },
            new Vehiculo { Placa = "LMN-456", Marca = "Toyota",  Anio = 2015 }
        };

        // ===================== GET =====================

        // GET api/vehiculos  -> mostrar todos
        [HttpGet]
        public ActionResult<List<Vehiculo>> GetTodos()
        {
            return Ok(vehiculos);
        }

        // GET api/vehiculos/cantidad  -> contar vehiculos
        [HttpGet("cantidad")]
        public ActionResult<int> GetCantidad()
        {
            return Ok(vehiculos.Count());
        }

        // GET api/vehiculos/ABC-123  -> buscar por placa
        [HttpGet("{placa}")]
        public ActionResult<Vehiculo> GetPorPlaca(string placa)
        {
            Vehiculo? vehiculo = vehiculos
                .FirstOrDefault(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound("No existe un vehiculo con esa placa.");
            }

            return Ok(vehiculo);
        }

        // GET api/vehiculos/marca/Toyota  -> buscar por marca (Where)
        [HttpGet("marca/{marca}")]
        public ActionResult<List<Vehiculo>> GetPorMarca(string marca)
        {
            List<Vehiculo> encontrados = vehiculos
                .Where(v => v.Marca.ToLower() == marca.ToLower())
                .ToList();

            return Ok(encontrados);
        }

        // GET api/vehiculos/desde/2018  -> buscar desde un anio (Where)
        [HttpGet("desde/{anio:int}")]
        public ActionResult<List<Vehiculo>> GetDesdeAnio(int anio)
        {
            List<Vehiculo> encontrados = vehiculos
                .Where(v => v.Anio >= anio)
                .ToList();

            return Ok(encontrados);
        }

        // ===================== POST =====================

        // POST api/vehiculos  -> registrar un vehiculo nuevo
        [HttpPost]
        public ActionResult<Vehiculo> Post(Vehiculo nuevo)
        {
            // No permitir placas repetidas
            if (vehiculos.Any(v => v.Placa.ToLower() == nuevo.Placa.ToLower()))
            {
                return Conflict("Ya existe un vehiculo con esa placa.");
            }

            vehiculos.Add(nuevo);

            // 201 Created + ruta para consultar el vehiculo creado
            return CreatedAtAction(nameof(GetPorPlaca), new { placa = nuevo.Placa }, nuevo);
        }

        // ===================== PUT =====================

        // PUT api/vehiculos/ABC-123  -> reemplazar TODOS los datos
        [HttpPut("{placa}")]
        public ActionResult<Vehiculo> Put(string placa, Vehiculo datos)
        {
            Vehiculo? vehiculo = vehiculos
                .FirstOrDefault(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound("No existe un vehiculo con esa placa.");
            }

            vehiculo.Marca = datos.Marca;
            vehiculo.Anio = datos.Anio;

            return Ok(vehiculo);
        }

        // ===================== PATCH =====================

        // PATCH api/vehiculos/ABC-123  -> cambiar SOLO algunos datos
        [HttpPatch("{placa}")]
        public ActionResult<Vehiculo> Patch(string placa, VehiculoPatch cambios)
        {
            Vehiculo? vehiculo = vehiculos
                .FirstOrDefault(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound("No existe un vehiculo con esa placa.");
            }

            if (cambios.Marca != null)
            {
                vehiculo.Marca = cambios.Marca;
            }

            if (cambios.Anio != null)
            {
                vehiculo.Anio = cambios.Anio.Value;
            }

            return Ok(vehiculo);
        }

        // ===================== DELETE =====================

        // DELETE api/vehiculos/ABC-123  -> eliminar un vehiculo
        [HttpDelete("{placa}")]
        public IActionResult Delete(string placa)
        {
            Vehiculo? vehiculo = vehiculos
                .FirstOrDefault(v => v.Placa.ToLower() == placa.ToLower());

            if (vehiculo == null)
            {
                return NotFound("No existe un vehiculo con esa placa.");
            }

            vehiculos.Remove(vehiculo);

            return NoContent(); // 204: eliminado, sin contenido
        }
    }
}