using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Models;
using Hotel.DTOs;
using Hotel.Services;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todo el controlador
    public class TemporadaPrecioController : ControllerBase
    {
        private readonly ITemporadaPrecioService _temporadaPrecioService;

        public TemporadaPrecioController(ITemporadaPrecioService temporadaPrecioService)
        {
            _temporadaPrecioService = temporadaPrecioService;
        }

        // GET: api/TemporadaPrecio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemporadaPrecio>>> ObtenerTodos()
        {
            try
            {
                var temporadas = await _temporadaPrecioService.ObtenerTodosAsync();
                return Ok(temporadas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las temporadas", error = ex.Message });
            }
        }

        // GET: api/TemporadaPrecio/activos
        [HttpGet("activos")]
        [AllowAnonymous] // Este endpoint puede ser público para consultar disponibilidad
        public async Task<ActionResult<IEnumerable<TemporadaPrecio>>> ObtenerActivos()
        {
            try
            {
                var temporadas = await _temporadaPrecioService.ObtenerActivosAsync();
                return Ok(temporadas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las temporadas activas", error = ex.Message });
            }
        }

        // GET: api/TemporadaPrecio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemporadaPrecio>> ObtenerPorId(int id)
        {
            try
            {
                var temporada = await _temporadaPrecioService.ObtenerPorIdAsync(id);
                
                if (temporada == null)
                {
                    return NotFound(new { mensaje = $"Temporada con ID {id} no encontrada" });
                }

                return Ok(temporada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la temporada", error = ex.Message });
            }
        }

        // GET: api/TemporadaPrecio/fecha/2024-12-25
        [HttpGet("fecha/{fecha}")]
        [AllowAnonymous] // Público para que clientes puedan consultar precios
        public async Task<ActionResult<TemporadaPrecio>> ObtenerPorFecha(DateTime fecha)
        {
            try
            {
                var temporada = await _temporadaPrecioService.ObtenerPorFechaAsync(fecha);
                
                if (temporada == null)
                {
                    return NotFound(new { mensaje = $"No hay temporada activa para la fecha {fecha:yyyy-MM-dd}" });
                }

                return Ok(temporada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la temporada", error = ex.Message });
            }
        }

        // POST: api/TemporadaPrecio
        [HttpPost]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<TemporadaPrecio>> Crear([FromBody] TemporadaPrecioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var temporada = new TemporadaPrecio
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    FechaInicio = dto.FechaInicio,
                    FechaFin = dto.FechaFin,
                    FactorMultiplicador = dto.FactorMultiplicador,
                    Activo = dto.Activo
                };

                var temporadaCreada = await _temporadaPrecioService.CrearAsync(temporada);
                
                return CreatedAtAction(
                    nameof(ObtenerPorId), 
                    new { id = temporadaCreada.Id }, 
                    temporadaCreada
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la temporada", error = ex.Message });
            }
        }

        // PUT: api/TemporadaPrecio/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<TemporadaPrecio>> Actualizar(int id, [FromBody] TemporadaPrecioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var temporada = new TemporadaPrecio
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    FechaInicio = dto.FechaInicio,
                    FechaFin = dto.FechaFin,
                    FactorMultiplicador = dto.FactorMultiplicador,
                    Activo = dto.Activo
                };

                var temporadaActualizada = await _temporadaPrecioService.ActualizarAsync(id, temporada);
                
                if (temporadaActualizada == null)
                {
                    return NotFound(new { mensaje = $"Temporada con ID {id} no encontrada" });
                }

                return Ok(temporadaActualizada);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la temporada", error = ex.Message });
            }
        }

        // DELETE: api/TemporadaPrecio/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var eliminado = await _temporadaPrecioService.EliminarAsync(id);
                
                if (!eliminado)
                {
                    return NotFound(new { mensaje = $"Temporada con ID {id} no encontrada" });
                }

                return Ok(new { mensaje = "Temporada eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la temporada", error = ex.Message });
            }
        }

        // PATCH: api/TemporadaPrecio/5/activar
        [HttpPatch("{id}/activar")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult> ActivarDesactivar(int id, [FromBody] bool activo)
        {
            try
            {
                var resultado = await _temporadaPrecioService.ActivarDesactivarAsync(id, activo);
                
                if (!resultado)
                {
                    return NotFound(new { mensaje = $"Temporada con ID {id} no encontrada" });
                }

                var mensaje = activo ? "activada" : "desactivada";
                return Ok(new { mensaje = $"Temporada {mensaje} correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cambiar el estado de la temporada", error = ex.Message });
            }
        }

        // GET: api/TemporadaPrecio/validar-traslape?fechaInicio=2024-12-01&fechaFin=2024-12-31
        [HttpGet("validar-traslape")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<bool>> ValidarTraslape(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin,
            [FromQuery] int? temporadaIdExcluir = null)
        {
            try
            {
                var existeTraslape = await _temporadaPrecioService.ExisteTraslapeAsync(
                    fechaInicio, 
                    fechaFin, 
                    temporadaIdExcluir
                );
                
                return Ok(new { existeTraslape });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al validar traslape", error = ex.Message });
            }
        }
    }
}