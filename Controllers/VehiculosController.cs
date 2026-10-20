using Microsoft.AspNetCore.Mvc;
using TallerMecanicoAPI.Data;  
using TallerMecanicoAPI.Models;
namespace TallerMecanicoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly VehiculoDatos _vehiculoDatos;

        public VehiculosController(VehiculoDatos vehiculoDatos)
        {
            _vehiculoDatos = vehiculoDatos;
        }

        [HttpGet]
        public async Task<ActionResult<List<Vehiculo>>> Get()
        {
            var lista = await _vehiculoDatos.ListarAsync();
            return Ok(lista);
        }

        [HttpGet("{placa}")]
        public async Task<ActionResult<Vehiculo>> Get(string placa)
        {
            var vehiculo = await _vehiculoDatos.ObtenerPorPlacaAsync(placa);
            if (vehiculo == null)
            {
                return NotFound($"Vehículo con placa {placa} no encontrado.");
            }
            return Ok(vehiculo);
        }

        [HttpPost]
        public async Task<ActionResult<Vehiculo>> Post([FromBody] Vehiculo nuevoVehiculo)
        {
            bool registrado = await _vehiculoDatos.RegistrarAsync(nuevoVehiculo);
            if (!registrado)
            {
                return BadRequest("No se pudo registrar el vehículo.");
            }

            return CreatedAtAction(nameof(Get), new { placa = nuevoVehiculo.Placa }, nuevoVehiculo);
        }

        [HttpPatch("{placa}")]
        public async Task<IActionResult> Patch(string placa, [FromBody] Vehiculo cambios)
        {
            bool modificado = await _vehiculoDatos.ModificarAsync(placa, cambios);
            if (!modificado)
            {
                return NotFound($"No se pudo actualizar: placa {placa} inexistente o sin datos modificados.");
            }

            var vehiculoActualizado = await _vehiculoDatos.ObtenerPorPlacaAsync(placa);
            return Ok(vehiculoActualizado);
        }

        [HttpDelete("{placa}")]
        public async Task<IActionResult> Delete(string placa)
        {
            bool eliminado = await _vehiculoDatos.EliminarAsync(placa);
            if (!eliminado)
            {
                return NotFound($"Vehículo con placa {placa} no encontrado.");
            }

            return NoContent();
        }
    }
}