using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hotel.Models;
using Hotel.DTOs;
using Hotel.Services.Interfaces;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todo el controlador
    public class TemporadaHabitacionPrecioController : ControllerBase
    {
        private readonly ITemporadaHabitacionPrecioService _temporadaHabitacionPrecioService;

        public TemporadaHabitacionPrecioController(ITemporadaHabitacionPrecioService temporadaHabitacionPrecioService)
        {
            _temporadaHabitacionPrecioService = temporadaHabitacionPrecioService;
        }

        // GET: api/TemporadaHabitacionPrecio
        [HttpGet]
        [Authorize(Roles = "admin")] // Solo administradores ven todos
        public async Task<ActionResult<IEnumerable<TemporadaHabitacionPrecio>>> ObtenerTodos()
        {
            try
            {
                var precios = await _temporadaHabitacionPrecioService.ObtenerTodosAsync();
                return Ok(precios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los precios", error = ex.Message });
            }
        }

        // GET: api/TemporadaHabitacionPrecio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemporadaHabitacionPrecio>> ObtenerPorId(int id)
        {
            try
            {
                var precio = await _temporadaHabitacionPrecioService.ObtenerPorIdAsync(id);
                
                if (precio == null)
                {
                    return NotFound(new { mensaje = $"Precio con ID {id} no encontrado" });
                }

                return Ok(precio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el precio", error = ex.Message });
            }
        }

        // GET: api/TemporadaHabitacionPrecio/temporada/5
        [HttpGet("temporada/{temporadaId}")]
        public async Task<ActionResult<IEnumerable<TemporadaHabitacionPrecio>>> ObtenerPorTemporada(int temporadaId)
        {
            try
            {
                var precios = await _temporadaHabitacionPrecioService.ObtenerPorTemporadaAsync(temporadaId);
                return Ok(precios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los precios de la temporada", error = ex.Message });
            }
        }

        // GET: api/TemporadaHabitacionPrecio/habitacion/5
        [HttpGet("habitacion/{habitacionId}")]
        public async Task<ActionResult<IEnumerable<TemporadaHabitacionPrecio>>> ObtenerPorHabitacion(int habitacionId)
        {
            try
            {
                var precios = await _temporadaHabitacionPrecioService.ObtenerPorHabitacionAsync(habitacionId);
                return Ok(precios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los precios de la habitación", error = ex.Message });
            }
        }

        // GET: api/TemporadaHabitacionPrecio/temporada/5/habitacion/10
        [HttpGet("temporada/{temporadaId}/habitacion/{habitacionId}")]
        public async Task<ActionResult<TemporadaHabitacionPrecio>> ObtenerPorTemporadaYHabitacion(
            int temporadaId, 
            int habitacionId)
        {
            try
            {
                var precio = await _temporadaHabitacionPrecioService
                    .ObtenerPorTemporadaYHabitacionAsync(temporadaId, habitacionId);
                
                if (precio == null)
                {
                    return NotFound(new { 
                        mensaje = $"No se encontró precio para la temporada {temporadaId} y habitación {habitacionId}" 
                    });
                }

                return Ok(precio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el precio", error = ex.Message });
            }
        }

        // GET: api/TemporadaHabitacionPrecio/calcular/habitacion/5?fecha=2024-12-25
        [HttpGet("calcular/habitacion/{habitacionId}")]
        [AllowAnonymous] // Público para que clientes consulten precios antes de reservar
        public async Task<ActionResult<decimal>> ObtenerPrecioCalculado(
            int habitacionId, 
            [FromQuery] DateTime fecha)
        {
            try
            {
                var precio = await _temporadaHabitacionPrecioService
                    .ObtenerPrecioHabitacionAsync(habitacionId, fecha);
                
                return Ok(new { 
                    habitacionId, 
                    fecha = fecha.ToString("yyyy-MM-dd"),
                    precio 
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al calcular el precio", error = ex.Message });
            }
        }

        // POST: api/TemporadaHabitacionPrecio
        [HttpPost]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<TemporadaHabitacionPrecio>> Crear(
            [FromBody] TemporadaHabitacionPrecioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var temporadaHabitacionPrecio = new TemporadaHabitacionPrecio
                {
                    TemporadaId = dto.TemporadaId,
                    HabitacionId = dto.HabitacionId,
                    PrecioOverride = dto.PrecioOverride
                };

                var precioCreado = await _temporadaHabitacionPrecioService
                    .CrearAsync(temporadaHabitacionPrecio);
                
                return CreatedAtAction(
                    nameof(ObtenerPorId), 
                    new { id = precioCreado.Id }, 
                    precioCreado
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el precio", error = ex.Message });
            }
        }

        // POST: api/TemporadaHabitacionPrecio/multiples/temporada/5
        [HttpPost("multiples/temporada/{temporadaId}")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<IEnumerable<TemporadaHabitacionPrecio>>> CrearMultiples(
            int temporadaId,
            [FromBody] List<TemporadaHabitacionPrecioCreateMultipleDto> preciosDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var precios = preciosDto.Select(dto => new TemporadaHabitacionPrecio
                {
                    TemporadaId = temporadaId,
                    HabitacionId = dto.HabitacionId,
                    PrecioOverride = dto.PrecioOverride
                }).ToList();

                var preciosCreados = await _temporadaHabitacionPrecioService
                    .CrearMultiplesAsync(temporadaId, precios);
                
                return Ok(new { 
                    mensaje = $"Se crearon {preciosCreados.Count()} precios correctamente",
                    precios = preciosCreados 
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear los precios", error = ex.Message });
            }
        }

        // PUT: api/TemporadaHabitacionPrecio/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<TemporadaHabitacionPrecio>> Actualizar(
            int id, 
            [FromBody] TemporadaHabitacionPrecioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var temporadaHabitacionPrecio = new TemporadaHabitacionPrecio
                {
                    TemporadaId = dto.TemporadaId,
                    HabitacionId = dto.HabitacionId,
                    PrecioOverride = dto.PrecioOverride
                };

                var precioActualizado = await _temporadaHabitacionPrecioService
                    .ActualizarAsync(id, temporadaHabitacionPrecio);
                
                if (precioActualizado == null)
                {
                    return NotFound(new { mensaje = $"Precio con ID {id} no encontrado" });
                }

                return Ok(precioActualizado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el precio", error = ex.Message });
            }
        }

        // DELETE: api/TemporadaHabitacionPrecio/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                var eliminado = await _temporadaHabitacionPrecioService.EliminarAsync(id);
                
                if (!eliminado)
                {
                    return NotFound(new { mensaje = $"Precio con ID {id} no encontrado" });
                }

                return Ok(new { mensaje = "Precio eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el precio", error = ex.Message });
            }
        }

        // DELETE: api/TemporadaHabitacionPrecio/temporada/5
        [HttpDelete("temporada/{temporadaId}")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult> EliminarPorTemporada(int temporadaId)
        {
            try
            {
                var eliminado = await _temporadaHabitacionPrecioService
                    .EliminarPorTemporadaAsync(temporadaId);
                
                if (!eliminado)
                {
                    return NotFound(new { 
                        mensaje = $"No se encontraron precios para la temporada {temporadaId}" 
                    });
                }

                return Ok(new { mensaje = "Precios eliminados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar los precios", error = ex.Message });
            }
        }

        // GET: api/TemporadaHabitacionPrecio/existe?temporadaId=5&habitacionId=10
        [HttpGet("existe")]
        [Authorize(Roles = "admin")] // Solo administradores
        public async Task<ActionResult<bool>> ExistePrecio(
            [FromQuery] int temporadaId,
            [FromQuery] int habitacionId,
            [FromQuery] int? idExcluir = null)
        {
            try
            {
                var existe = await _temporadaHabitacionPrecioService
                    .ExistePrecioAsync(temporadaId, habitacionId, idExcluir);
                
                return Ok(new { existe });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al validar existencia", error = ex.Message });
            }
        }
    }
}