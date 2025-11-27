using Hotel.Data;
using Hotel.Models;
using ProjectHotel.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Hotel.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly HotelDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _fotosPath;

        public HabitacionService(HotelDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            // Crear carpeta para fotos si no existe
            _fotosPath = Path.Combine(_environment.WebRootPath, "uploads", "habitaciones");
            if (!Directory.Exists(_fotosPath))
            {
                Directory.CreateDirectory(_fotosPath);
            }
        }

        // Método auxiliar para guardar archivo
        private async Task<string> GuardarArchivoAsync(IFormFile archivo)
        {
            // Generar nombre único
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(_fotosPath, nombreArchivo);

            // Guardar archivo
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Retornar ruta relativa para la URL
            return $"/uploads/habitaciones/{nombreArchivo}";
        }

        // Método auxiliar para eliminar archivo
        private void EliminarArchivo(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return;
                
                // Convertir URL a ruta física
                var nombreArchivo = Path.GetFileName(url);
                var rutaCompleta = Path.Combine(_fotosPath, nombreArchivo);

                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
            }
            catch (Exception ex)
            {
                // Log error pero no interrumpir el flujo
                Console.WriteLine($"Error al eliminar archivo: {ex.Message}");
            }
        }

        public async Task<Habitacion> CrearHabitacionAsync(
            Habitacion habitacion, 
            List<int>? serviciosIds, 
            List<IFormFile>? fotos)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verificar que el número de habitación no exista
                var existeNumero = await _context.Habitaciones
                    .AnyAsync(h => h.NumeroHabitacion == habitacion.NumeroHabitacion);

                if (existeNumero)
                    throw new Exception($"Ya existe una habitación con el número {habitacion.NumeroHabitacion}");

                // Verificar que el tipo de habitación existe
                var tipoExiste = await _context.TiposHabitacion
                    .AnyAsync(t => t.Id == habitacion.TipoHabitacionId);

                if (!tipoExiste)
                    throw new Exception("El tipo de habitación especificado no existe");

                // Configurar estado inicial
                habitacion.Estado = "disponible";
                habitacion.CreadoEn = DateTime.UtcNow;
                habitacion.ActualizadoEn = DateTime.UtcNow;

                _context.Habitaciones.Add(habitacion);
                await _context.SaveChangesAsync();

                // Agregar servicios si existen
                if (serviciosIds != null && serviciosIds.Any())
                {
                    foreach (var servicioId in serviciosIds)
                    {
                        var servicioExiste = await _context.Servicios.AnyAsync(s => s.Id == servicioId);
                        if (servicioExiste)
                        {
                            _context.HabitacionServicios.Add(new HabitacionServicio
                            {
                                HabitacionId = habitacion.Id,
                                ServicioId = servicioId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Guardar fotos si existen
                if (fotos != null && fotos.Any())
                {
                    bool esPrimeraFoto = true;
                    foreach (var archivoFoto in fotos)
                    {
                        // Guardar archivo y obtener URL
                        var urlFoto = await GuardarArchivoAsync(archivoFoto);

                        var foto = new HabitacionFoto
                        {
                            HabitacionId = habitacion.Id,
                            Url = urlFoto,
                            EsPrincipal = esPrimeraFoto, // Primera foto es principal por defecto
                            CreadoEn = DateTime.UtcNow
                        };

                        _context.HabitacionFotos.Add(foto);
                        esPrimeraFoto = false;
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return habitacion;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Habitacion?> ActualizarHabitacionAsync(
            int id, 
            ActualizarHabitacionRequest request, 
            List<IFormFile>? nuevasFotos = null)
        {
            Console.WriteLine($"=== SERVICE: ActualizarHabitacionAsync ===");
            Console.WriteLine($"ID: {id}");
            Console.WriteLine($"NuevasFotos recibidas: {nuevasFotos?.Count ?? 0}");

            var habitacion = await _context.Habitaciones
                .Include(h => h.HabitacionServicios)
                .Include(h => h.Fotos)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habitacion == null)
            {
                Console.WriteLine("Habitación no encontrada");
                return null;
            }

            Console.WriteLine($"Fotos actuales de la habitación: {habitacion.Fotos.Count}");

            // Actualizar campos si se proporcionan
            if (!string.IsNullOrEmpty(request.NumeroHabitacion))
            {
                var existeNumero = await _context.Habitaciones
                    .AnyAsync(h => h.NumeroHabitacion == request.NumeroHabitacion && h.Id != id);

                if (existeNumero)
                    throw new Exception($"Ya existe otra habitación con el número {request.NumeroHabitacion}");

                habitacion.NumeroHabitacion = request.NumeroHabitacion;
            }

            if (request.TipoHabitacionId.HasValue)
            {
                var tipoExiste = await _context.TiposHabitacion
                    .AnyAsync(t => t.Id == request.TipoHabitacionId.Value);

                if (!tipoExiste)
                    throw new Exception("El tipo de habitación especificado no existe");

                habitacion.TipoHabitacionId = request.TipoHabitacionId.Value;
            }

            if (request.Piso.HasValue)
                habitacion.Piso = request.Piso.Value;

            if (request.PrecioBase.HasValue)
                habitacion.PrecioBase = request.PrecioBase.Value;

            if (request.Capacidad.HasValue)
                habitacion.Capacidad = request.Capacidad.Value;

            if (request.Descripcion != null)
                habitacion.Descripcion = request.Descripcion;

            if (!string.IsNullOrEmpty(request.Estado))
                habitacion.Estado = request.Estado.ToLower();

            habitacion.ActualizadoEn = DateTime.UtcNow;

            // Actualizar servicios si se proporcionan
            if (request.ServiciosIds != null)
            {
                Console.WriteLine($"Actualizando servicios: {request.ServiciosIds.Count}");
                // Eliminar servicios actuales
                _context.HabitacionServicios.RemoveRange(habitacion.HabitacionServicios);

                // Agregar nuevos servicios
                foreach (var servicioId in request.ServiciosIds)
                {
                    var servicioExiste = await _context.Servicios.AnyAsync(s => s.Id == servicioId);
                    if (servicioExiste)
                    {
                        _context.HabitacionServicios.Add(new HabitacionServicio
                        {
                            HabitacionId = habitacion.Id,
                            ServicioId = servicioId
                        });
                    }
                }
            }

            // Manejar nuevas fotos
            if (nuevasFotos != null && nuevasFotos.Any())
            {
                Console.WriteLine($"Procesando {nuevasFotos.Count} nuevas fotos");
                
                // Si se debe reemplazar todas las fotos
                if (request.ReemplazarFotos == true)
                {
                    Console.WriteLine("ReemplazarFotos = true, eliminando fotos existentes");
                    // Eliminar fotos existentes (archivos y registros)
                    foreach (var foto in habitacion.Fotos.ToList())
                    {
                        EliminarArchivo(foto.Url);
                        _context.HabitacionFotos.Remove(foto);
                    }
                    await _context.SaveChangesAsync();
                    
                    // Recargar fotos
                    await _context.Entry(habitacion).Collection(h => h.Fotos).LoadAsync();
                    Console.WriteLine($"Fotos después de eliminar: {habitacion.Fotos.Count}");
                }

                // Agregar nuevas fotos
                bool esPrimeraFoto = !habitacion.Fotos.Any();
                Console.WriteLine($"esPrimeraFoto: {esPrimeraFoto}");
                
                foreach (var archivoFoto in nuevasFotos)
                {
                    Console.WriteLine($"Guardando archivo: {archivoFoto.FileName}");
                    var urlFoto = await GuardarArchivoAsync(archivoFoto);
                    Console.WriteLine($"URL generada: {urlFoto}");

                    var foto = new HabitacionFoto
                    {
                        HabitacionId = habitacion.Id,
                        Url = urlFoto,
                        EsPrincipal = esPrimeraFoto,
                        CreadoEn = DateTime.UtcNow
                    };

                    _context.HabitacionFotos.Add(foto);
                    esPrimeraFoto = false;
                }
                
                await _context.SaveChangesAsync();
                Console.WriteLine("Fotos guardadas en la base de datos");
                
                // Recargar fotos después de guardar
                await _context.Entry(habitacion).Collection(h => h.Fotos).LoadAsync();
                Console.WriteLine($"Total de fotos después de guardar: {habitacion.Fotos.Count}");
            }
            else
            {
                Console.WriteLine("No se recibieron nuevas fotos");
            }

            await _context.SaveChangesAsync();
            return habitacion;
            
        }

        public async Task<FotoResponse> AgregarFotoAsync(
            int habitacionId, 
            IFormFile foto, 
            string? descripcion = null, 
            bool esPrincipal = false)
        {
            var habitacion = await _context.Habitaciones
                .Include(h => h.Fotos)
                .FirstOrDefaultAsync(h => h.Id == habitacionId);

            if (habitacion == null)
                throw new Exception("Habitación no encontrada");

            // Si es principal, quitar la principal actual
            if (esPrincipal)
            {
                var fotoActualPrincipal = habitacion.Fotos.FirstOrDefault(f => f.EsPrincipal);
                if (fotoActualPrincipal != null)
                {
                    fotoActualPrincipal.EsPrincipal = false;
                }
            }

            // Guardar archivo
            var urlFoto = await GuardarArchivoAsync(foto);

            var nuevaFoto = new HabitacionFoto
            {
                HabitacionId = habitacionId,
                Url = urlFoto,
                Descripcion = descripcion,
                EsPrincipal = esPrincipal,
                CreadoEn = DateTime.UtcNow
            };

            _context.HabitacionFotos.Add(nuevaFoto);
            await _context.SaveChangesAsync();

            return new FotoResponse
            {
                Id = nuevaFoto.Id,
                Url = nuevaFoto.Url,
                Descripcion = nuevaFoto.Descripcion,
                EsPrincipal = nuevaFoto.EsPrincipal
            };
        }

        public async Task<bool> EliminarFotoAsync(int fotoId)
        {
            var foto = await _context.HabitacionFotos.FindAsync(fotoId);
            if (foto == null)
                return false;

            // Eliminar archivo físico
            EliminarArchivo(foto.Url);

            // Eliminar registro
            _context.HabitacionFotos.Remove(foto);
            await _context.SaveChangesAsync();

            return true;
        }

        // ... resto de los métodos (ObtenerDetalleAsync, ObtenerTodasConDetalleAsync, etc.)
        // se mantienen igual que antes
        
        public async Task<HabitacionDetalleResponse?> ObtenerDetalleAsync(int id)
        {
            var habitacion = await _context.Habitaciones
                .Include(h => h.TipoHabitacion)
                .Include(h => h.Fotos)
                .Include(h => h.HabitacionServicios)
                    .ThenInclude(hs => hs.Servicio)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (habitacion == null)
                return null;

            return MapToDetalleResponse(habitacion);
        }

        public async Task<List<HabitacionDetalleResponse>> ObtenerTodasConDetalleAsync()
        {
            var habitaciones = await _context.Habitaciones
                .Include(h => h.TipoHabitacion)
                .Include(h => h.Fotos)
                .Include(h => h.HabitacionServicios)
                    .ThenInclude(hs => hs.Servicio)
                .ToListAsync();

            return habitaciones.Select(MapToDetalleResponse).ToList();
        }

        // Método auxiliar para mapear
        private HabitacionDetalleResponse MapToDetalleResponse(Habitacion habitacion)
        {
            return new HabitacionDetalleResponse
            {
                Id = habitacion.Id,
                NumeroHabitacion = habitacion.NumeroHabitacion,
                Piso = habitacion.Piso ?? 0,
                PrecioBase = habitacion.PrecioBase,
                Capacidad = habitacion.Capacidad,
                Descripcion = habitacion.Descripcion,
                Estado = habitacion.Estado,
                TipoHabitacionId = habitacion.TipoHabitacionId,
                TipoHabitacion = habitacion.TipoHabitacion != null ? new TipoHabitacionResponse
                {
                    Id = habitacion.TipoHabitacion.Id,
                    Nombre = habitacion.TipoHabitacion.Nombre,
                    Descripcion = habitacion.TipoHabitacion.Descripcion
                } : null,
                Fotos = habitacion.Fotos.Select(f => new FotoResponse
                {
                    Id = f.Id,
                    Url = f.Url,
                    Descripcion = f.Descripcion,
                    EsPrincipal = f.EsPrincipal
                }).ToList(),
                Servicios = habitacion.HabitacionServicios.Select(hs => new ServicioResponse
                {
                    Id = hs.Servicio.Id,
                    Nombre = hs.Servicio.Nombre,
                    Descripcion = hs.Servicio.Descripcion,
                }).ToList(),
                CreadoEn = habitacion.CreadoEn,
                ActualizadoEn = habitacion.ActualizadoEn
            };
        }

        // Implementar métodos restantes...
        public Task<HabitacionDetalleResponse?> ObtenerPorNumeroAsync(string numeroHabitacion)
        {
            throw new NotImplementedException();
        }

        public Task<List<HabitacionDetalleResponse>> ObtenerDisponiblesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<HabitacionDetalleResponse>> FiltrarHabitacionesAsync(FiltroHabitacionesRequest filtro)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CambiarEstadoAsync(int id, string estado)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarHabitacionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task AgregarServicioAsync(int habitacionId, int servicioId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarServicioAsync(int habitacionId, int servicioId)
        {
            throw new NotImplementedException();
        }
    }
}