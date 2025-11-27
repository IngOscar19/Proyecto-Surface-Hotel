using Hotel.Models;
using Hotel.Services;
using Hotel.Validators;
using ProjectHotel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitacionesController : ControllerBase
    {
        private readonly IHabitacionService _habitacionService;
        private readonly CrearHabitacionValidator _crearValidator;
        private readonly ActualizarHabitacionValidator _actualizarValidator;
        private readonly AgregarFotoValidator _fotoValidator;

        public HabitacionesController(
            IHabitacionService habitacionService,
            CrearHabitacionValidator crearValidator,
            ActualizarHabitacionValidator actualizarValidator,
            AgregarFotoValidator fotoValidator)
        {
            _habitacionService = habitacionService;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
            _fotoValidator = fotoValidator;
        }

        // POST: api/habitaciones - Crear habitación con archivos (FormData)
        [Authorize(Roles = "admin,empleado")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] CrearHabitacionFormRequest formRequest)
        {
            // Convertir FormRequest a Request normal para validación
            var request = new CrearHabitacionRequest
            {
                NumeroHabitacion = formRequest.NumeroHabitacion,
                TipoHabitacionId = formRequest.TipoHabitacionId,
                Piso = formRequest.Piso,
                PrecioBase = formRequest.PrecioBase,
                Capacidad = formRequest.Capacidad,
                Descripcion = formRequest.Descripcion,
                ServiciosIds = formRequest.ServiciosIds
            };

            var validationResult = await _crearValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    errores = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            // Validar archivos de fotos si existen
            if (formRequest.Fotos != null && formRequest.Fotos.Any())
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                foreach (var foto in formRequest.Fotos)
                {
                    var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();
                    if (!extensionesPermitidas.Contains(extension))
                    {
                        return BadRequest(new { mensaje = $"Archivo {foto.FileName}: Solo se permiten imágenes JPG, PNG o WEBP" });
                    }

                    if (foto.Length > 5 * 1024 * 1024) // 5MB
                    {
                        return BadRequest(new { mensaje = $"Archivo {foto.FileName}: El tamaño no puede superar los 5MB" });
                    }
                }
            }

            try
            {
                var habitacion = new Habitacion
                {
                    NumeroHabitacion = formRequest.NumeroHabitacion,
                    TipoHabitacionId = formRequest.TipoHabitacionId,
                    Piso = formRequest.Piso,
                    PrecioBase = formRequest.PrecioBase,
                    Capacidad = formRequest.Capacidad,
                    Descripcion = formRequest.Descripcion
                };

                // Pasar archivos IFormFile al servicio
                var resultado = await _habitacionService.CrearHabitacionAsync(
                    habitacion, 
                    formRequest.ServiciosIds,
                    formRequest.Fotos  // Lista de IFormFile
                );
                
                var detalle = await _habitacionService.ObtenerDetalleAsync(resultado.Id);
                
                return CreatedAtAction(
                    nameof(ObtenerPorId), 
                    new { id = resultado.Id }, 
                    new 
                    { 
                        mensaje = "Habitación creada correctamente", 
                        habitacion = detalle 
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/habitaciones - Obtener todas las habitaciones con detalles
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var habitaciones = await _habitacionService.ObtenerTodasConDetalleAsync();
                return Ok(habitaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/habitaciones/{id} - Obtener habitación por ID con detalle
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var habitacion = await _habitacionService.ObtenerDetalleAsync(id);
                if (habitacion == null)
                    return NotFound(new { mensaje = "Habitación no encontrada" });

                return Ok(habitacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/habitaciones/numero/{numero} - Obtener habitación por número
        [HttpGet("numero/{numero}")]
        public async Task<IActionResult> ObtenerPorNumero(string numero)
        {
            try
            {
                var habitacion = await _habitacionService.ObtenerPorNumeroAsync(numero);
                if (habitacion == null)
                    return NotFound(new { mensaje = "Habitación no encontrada" });

                return Ok(habitacion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/habitaciones/disponibles - Obtener habitaciones disponibles
        [HttpGet("disponibles")]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            try
            {
                var habitaciones = await _habitacionService.ObtenerDisponiblesAsync();
                return Ok(habitaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/habitaciones/filtrar - Filtrar habitaciones
        [HttpPost("filtrar")]
        public async Task<IActionResult> Filtrar([FromBody] FiltroHabitacionesRequest filtro)
        {
            try
            {
                var habitaciones = await _habitacionService.FiltrarHabitacionesAsync(filtro);
                return Ok(habitaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/habitaciones/{id} - Actualizar habitación con archivos (FormData)
        [Authorize(Roles = "admin,empleado")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromForm] ActualizarHabitacionFormRequest formRequest)
        {

            Console.WriteLine($"formRequest es null: {formRequest == null}");
            Console.WriteLine($"formRequest.NuevasFotos es null: {formRequest.NuevasFotos == null}");
            Console.WriteLine($"formRequest.NuevasFotos count: {formRequest.NuevasFotos?.Count}");
            // LOG para debug
            Console.WriteLine($"=== DEBUG ACTUALIZAR HABITACIÓN ===");
            Console.WriteLine($"ID: {id}");
            Console.WriteLine($"NuevasFotos count: {formRequest.NuevasFotos?.Count ?? 0}");
            if (formRequest.NuevasFotos != null)
            {
                foreach (var foto in formRequest.NuevasFotos)
                {
                    Console.WriteLine($"Foto recibida: {foto.FileName}, Tamaño: {foto.Length} bytes");
                }
            }

            // Convertir FormRequest a Request normal para validación
            var request = new ActualizarHabitacionRequest
            {
                NumeroHabitacion = formRequest.NumeroHabitacion,
                TipoHabitacionId = formRequest.TipoHabitacionId,
                Piso = formRequest.Piso,
                PrecioBase = formRequest.PrecioBase,
                Capacidad = formRequest.Capacidad,
                Descripcion = formRequest.Descripcion,
                Estado = formRequest.Estado,
                ServiciosIds = formRequest.ServiciosIds,
                ReemplazarFotos = formRequest.ReemplazarFotos
            };

            var validationResult = await _actualizarValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    errores = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            // Validar nuevas fotos si existen
            if (formRequest.NuevasFotos != null && formRequest.NuevasFotos.Any())
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                foreach (var foto in formRequest.NuevasFotos)
                {
                    var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();
                    if (!extensionesPermitidas.Contains(extension))
                    {
                        return BadRequest(new { mensaje = $"Archivo {foto.FileName}: Solo se permiten imágenes JPG, PNG o WEBP" });
                    }

                    if (foto.Length > 5 * 1024 * 1024) // 5MB
                    {
                        return BadRequest(new { mensaje = $"Archivo {foto.FileName}: El tamaño no puede superar los 5MB" });
                    }
                }
            }

            try
            {
                var resultado = await _habitacionService.ActualizarHabitacionAsync(
                    id, 
                    request, 
                    formRequest.NuevasFotos  // Lista de IFormFile
                );
                
                if (resultado == null)
                    return NotFound(new { mensaje = "Habitación no encontrada" });

                var detalle = await _habitacionService.ObtenerDetalleAsync(resultado.Id);

                Console.WriteLine($"Fotos en respuesta: {detalle?.Fotos?.Count ?? 0}");

                return Ok(new { mensaje = "Habitación actualizada correctamente", habitacion = detalle });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return BadRequest(new { mensaje = ex.Message });
            }
            
        }

        // PATCH: api/habitaciones/{id}/estado - Cambiar estado (solo admin/empleado)
        [Authorize(Roles = "admin,empleado")]
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request)
        {
            try
            {
                var estadosValidos = new[] { "disponible", "ocupada", "mantenimiento", "limpieza" };
                if (!estadosValidos.Contains(request.Estado.ToLower()))
                    return BadRequest(new { mensaje = "Estado inválido. Debe ser: disponible, ocupada, mantenimiento o limpieza" });

                var resultado = await _habitacionService.CambiarEstadoAsync(id, request.Estado);
                if (!resultado)
                    return NotFound(new { mensaje = "Habitación no encontrada" });

                return Ok(new { mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/habitaciones/{id} - Eliminar habitación (solo admin)
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _habitacionService.EliminarHabitacionAsync(id);
                if (!resultado)
                    return NotFound(new { mensaje = "Habitación no encontrada" });

                return Ok(new { mensaje = "Habitación eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/habitaciones/{id}/fotos - Agregar foto individual con archivo
        [Authorize(Roles = "admin,empleado")]
        [HttpPost("{id}/fotos")]
        public async Task<IActionResult> AgregarFoto(
            int id, 
            [FromForm] IFormFile foto, 
            [FromForm] string? descripcion = null, 
            [FromForm] bool esPrincipal = false)
        {
            if (foto == null || foto.Length == 0)
            {
                return BadRequest(new { mensaje = "Debe proporcionar un archivo de imagen" });
            }

            // Validar tipo de archivo
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();
            
            if (!extensionesPermitidas.Contains(extension))
            {
                return BadRequest(new { mensaje = "Solo se permiten archivos JPG, PNG o WEBP" });
            }

            // Validar tamaño (5MB)
            if (foto.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { mensaje = "El archivo no puede superar los 5MB" });
            }

            try
            {
                var fotoGuardada = await _habitacionService.AgregarFotoAsync(id, foto, descripcion, esPrincipal);
                return Ok(new { mensaje = "Foto agregada correctamente", foto = fotoGuardada });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/habitaciones/fotos/{fotoId} - Eliminar foto (solo admin/empleado)
        [Authorize(Roles = "admin,empleado")]
        [HttpDelete("fotos/{fotoId}")]
        public async Task<IActionResult> EliminarFoto(int fotoId)
        {
            try
            {
                var resultado = await _habitacionService.EliminarFotoAsync(fotoId);
                if (!resultado)
                    return NotFound(new { mensaje = "Foto no encontrada" });

                return Ok(new { mensaje = "Foto eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // POST: api/habitaciones/{habitacionId}/servicios/{servicioId} - Agregar servicio (solo admin/empleado)
        [Authorize(Roles = "admin,empleado")]
        [HttpPost("{habitacionId}/servicios/{servicioId}")]
        public async Task<IActionResult> AgregarServicio(int habitacionId, int servicioId)
        {
            try
            {
                await _habitacionService.AgregarServicioAsync(habitacionId, servicioId);
                return Ok(new { mensaje = "Servicio agregado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/habitaciones/{habitacionId}/servicios/{servicioId} - Eliminar servicio (solo admin/empleado)
        [Authorize(Roles = "admin,empleado")]
        [HttpDelete("{habitacionId}/servicios/{servicioId}")]
        public async Task<IActionResult> EliminarServicio(int habitacionId, int servicioId)
        {
            try
            {
                var resultado = await _habitacionService.EliminarServicioAsync(habitacionId, servicioId);
                if (!resultado)
                    return NotFound(new { mensaje = "Servicio no encontrado en esta habitación" });

                return Ok(new { mensaje = "Servicio eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}