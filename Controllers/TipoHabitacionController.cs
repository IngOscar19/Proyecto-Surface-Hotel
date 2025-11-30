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
    public class TiposHabitacionController : ControllerBase
    {
        private readonly ITipoHabitacionService _tipoHabitacionService;

        public TiposHabitacionController(ITipoHabitacionService tipoHabitacionService)
        {
            _tipoHabitacionService = tipoHabitacionService;
        }

        // GET: api/TiposHabitacion
        [HttpGet]
        [AllowAnonymous] // Permitir acceso público para consultas
        public async Task<ActionResult<IEnumerable<TipoHabitacion>>> ObtenerTodos()
        {
            try
            {
                var tipos = await _tipoHabitacionService.ObtenerTodosAsync();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los tipos de habitación", error = ex.Message });
            }
        }

        // GET: api/TiposHabitacion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoHabitacion>> ObtenerPorId(int id)
        {
            try
            {
                var tipo = await _tipoHabitacionService.ObtenerPorIdAsync(id);
                
                if (tipo == null)
                {
                    return NotFound(new { mensaje = $"Tipo de habitación con ID {id} no encontrado" });
                }

                return Ok(tipo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el tipo de habitación", error = ex.Message });
            }
        }

        // POST: api/TiposHabitacion
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<TipoHabitacion>> Crear([FromBody] TipoHabitacionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var tipoHabitacion = new TipoHabitacion
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    FactorTipo = dto.FactorTipo
                };

                var tipoCreado = await _tipoHabitacionService.CrearAsync(tipoHabitacion);
                
                return CreatedAtAction(
                    nameof(ObtenerPorId), 
                    new { id = tipoCreado.Id }, 
                    tipoCreado
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el tipo de habitación", error = ex.Message });
            }
        }

        // PUT: api/TiposHabitacion/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<TipoHabitacion>> Actualizar(int id, [FromBody] TipoHabitacionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var tipoHabitacion = new TipoHabitacion
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    FactorTipo = dto.FactorTipo
                };

                var tipoActualizado = await _tipoHabitacionService.ActualizarAsync(id, tipoHabitacion);
                
                if (tipoActualizado == null)
                {
                    return NotFound(new { mensaje = $"Tipo de habitación con ID {id} no encontrado" });
                }

                return Ok(tipoActualizado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el tipo de habitación", error = ex.Message });
            }
        }

        // DELETE: api/TiposHabitacion/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var eliminado = await _tipoHabitacionService.EliminarAsync(id);
                
                if (!eliminado)
                {
                    return NotFound(new { mensaje = $"Tipo de habitación con ID {id} no encontrado" });
                }

                return Ok(new { mensaje = "Tipo de habitación eliminado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el tipo de habitación", error = ex.Message });
            }
        }
    }
}