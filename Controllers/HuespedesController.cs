using Microsoft.AspNetCore.Mvc;
using Hotel.Services;
using ProjectHotel.DTOs;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuespedController : ControllerBase
    {
        private readonly IHuespedService _service;
        private readonly ILogger<HuespedController> _logger;

        public HuespedController(IHuespedService service, ILogger<HuespedController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearHuespedDTO dto)
        {
            try
            {
                _logger.LogInformation("=== CREAR HUÉSPED ===");
                _logger.LogInformation($"Nombre: {dto.Nombre}");
                _logger.LogInformation($"Apellido: {dto.Apellido}");
                _logger.LogInformation($"Email: {dto.Email}");
                _logger.LogInformation($"NumeroDocumento: {dto.NumeroDocumento}");

                var huesped = await _service.Crear(dto);
                
                _logger.LogInformation($"✅ Huésped creado con ID: {huesped.Id}");
                
                return Ok(huesped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear huésped: {Message}", ex.Message);
                
                return StatusCode(500, new
                {
                    mensaje = "Error al crear el huésped",
                    error = ex.Message,
                    detalleInterno = ex.InnerException?.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var lista = await _service.Listar();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar huéspedes: {Message}", ex.Message);
                return StatusCode(500, new { mensaje = "Error al listar huéspedes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            try
            {
                var h = await _service.Obtener(id);
                if (h == null) return NotFound(new { mensaje = "Huésped no encontrado" });
                return Ok(h);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener huésped {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { mensaje = "Error al obtener huésped", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CrearHuespedDTO dto)
        {
            try
            {
                var actualizado = await _service.Actualizar(id, dto);
                if (actualizado == null) return NotFound(new { mensaje = "Huésped no encontrado" });
                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar huésped {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { mensaje = "Error al actualizar huésped", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var eliminado = await _service.Eliminar(id);
                if (!eliminado) return NotFound(new { mensaje = "Huésped no encontrado" });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar huésped {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { mensaje = "Error al eliminar huésped", error = ex.Message });
            }
        }
    }
}