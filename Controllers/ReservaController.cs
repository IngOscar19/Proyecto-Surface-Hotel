using ProjectHotel.DTOs;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/reservas")]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly ILogger<ReservaController> _logger;

        public ReservaController(IReservaService reservaService, ILogger<ReservaController> logger)
        {
            _reservaService = reservaService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CrearReserva([FromBody] ReservaCreateDto dto)
        {
            try
            {
                // Validar que el claim existe
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                    ?? User.FindFirst("nameid") 
                    ?? User.FindFirst("sub");

                if (userIdClaim == null)
                {
                    _logger.LogError("No se encontró el claim de usuario en el token");
                    return Unauthorized(new { mensaje = "Token inválido o usuario no identificado" });
                }

                if (!int.TryParse(userIdClaim.Value, out int usuarioId))
                {
                    _logger.LogError($"El ID de usuario no es un número válido: {userIdClaim.Value}");
                    return BadRequest(new { mensaje = "ID de usuario inválido en el token" });
                }

                _logger.LogInformation($"Creando reserva para usuario {usuarioId}");
                
                var reserva = await _reservaService.CrearReservaAsync(dto, usuarioId);
                
                return Ok(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reserva: {Message}", ex.Message);
                return BadRequest(new { mensaje = ex.Message, detalle = ex.InnerException?.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerReserva(int id)
        {
            try
            {
                var reserva = await _reservaService.ObtenerReservaPorIdAsync(id);

                if (reserva == null)
                    return NotFound(new { mensaje = "Reserva no encontrada" });

                return Ok(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reserva {Id}: {Message}", id, ex.Message);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObtenerReservas()
        {
            try
            {
                var reservas = await _reservaService.ObtenerReservasAsync();
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas: {Message}", ex.Message);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("{id}/cancelar")]
        [Authorize]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                    ?? User.FindFirst("nameid") 
                    ?? User.FindFirst("sub");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
                {
                    return Unauthorized(new { mensaje = "Token inválido" });
                }

                var resultado = await _reservaService.CancelarReservaAsync(id, usuarioId);

                if (!resultado)
                    return NotFound(new { mensaje = "Reserva no encontrada" });

                return Ok(new { mensaje = "Reserva cancelada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar reserva {Id}: {Message}", id, ex.Message);
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}