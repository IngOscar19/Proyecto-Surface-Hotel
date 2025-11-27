using Hotel.Data;
using Hotel.Models;
using ProjectHotel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ServiciosController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/servicios - Obtener todos los servicios activos
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var servicios = await _context.Servicios
                    .Where(s => s.Activo)
                    .OrderBy(s => s.Nombre)
                    .ToListAsync();

                return Ok(servicios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/servicios/{id} - Obtener servicio por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio == null)
                    return NotFound(new { mensaje = "Servicio no encontrado" });

                return Ok(servicio);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/servicios - Crear servicio (solo admin)
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearServicioRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nombre))
                    return BadRequest(new { mensaje = "El nombre es obligatorio" });

                var servicio = new Servicio
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Icono = request.Icono,
                    Activo = true,
                    CreadoEn = DateTime.UtcNow
                };

                _context.Servicios.Add(servicio);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObtenerPorId), new { id = servicio.Id }, servicio);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/servicios/{id} - Actualizar servicio (solo admin)
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarServicioRequest request)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio == null)
                    return NotFound(new { mensaje = "Servicio no encontrado" });

                if (!string.IsNullOrEmpty(request.Nombre))
                    servicio.Nombre = request.Nombre;

                if (request.Descripcion != null)
                    servicio.Descripcion = request.Descripcion;

                if (request.Icono != null)
                    servicio.Icono = request.Icono;

                if (request.Activo.HasValue)
                    servicio.Activo = request.Activo.Value;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Servicio actualizado correctamente", servicio });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/servicios/{id} - Desactivar servicio (solo admin)
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Desactivar(int id)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio == null)
                    return NotFound(new { mensaje = "Servicio no encontrado" });

                servicio.Activo = false;
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Servicio desactivado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}