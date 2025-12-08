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
        // TEMPORAL: Quitado [Authorize] para pruebas
        public async Task<IActionResult> CrearReserva([FromBody] ReservaCreateDto dto)
        {
            try
            {
                // TEMPORAL: Usuario hardcodeado para pruebas
                int usuarioId = 1;

                _logger.LogInformation($"Creando reserva para usuario {usuarioId}");
                _logger.LogInformation($"HabitacionId: {dto.HabitacionId}");
                _logger.LogInformation($"HuespedId: {dto.HuespedId}");
                _logger.LogInformation($"FechaEntrada: {dto.FechaEntrada}");
                _logger.LogInformation($"FechaSalida: {dto.FechaSalida}");
                
                var reserva = await _reservaService.CrearReservaAsync(dto, usuarioId);
                
                _logger.LogInformation($"✅ Reserva creada con ID: {reserva.Id}");
                
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

        // ✅ NUEVO: Confirmar una reserva pendiente
        [HttpPatch("{id}/confirmar")]
        [Authorize(Roles = "admin,empleado")]
        public async Task<IActionResult> ConfirmarReserva(int id)
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

                var resultado = await _reservaService.ConfirmarReservaAsync(id, usuarioId);

                if (!resultado)
                    return NotFound(new { mensaje = "Reserva no encontrada" });

                return Ok(new { mensaje = "Reserva confirmada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar reserva {Id}: {Message}", id, ex.Message);
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