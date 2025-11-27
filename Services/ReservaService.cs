using Hotel.Data;
using Hotel.Models;
using ProjectHotel.DTOs;
using Microsoft.EntityFrameworkCore;
using Hotel.Services.Interfaces;

namespace Hotel.Services
{
    public class ReservaService : IReservaService
    {
        private readonly HotelDbContext _context;
        private readonly ITemporadaHabitacionPrecioService _precioService;

        public ReservaService(HotelDbContext context, ITemporadaHabitacionPrecioService precioService)
        {
            _context = context;
            _precioService = precioService;
        }

        public async Task<ReservaResponseDto> CrearReservaAsync(ReservaCreateDto dto, int usuarioId)
        {
            try
            {
                // Validar habitación
                var habitacion = await _context.Habitaciones
                    .Include(h => h.TipoHabitacion)
                    .FirstOrDefaultAsync(h => h.Id == dto.HabitacionId);

                if (habitacion == null)
                    throw new Exception("La habitación no existe.");

                if (habitacion.TipoHabitacion == null)
                    throw new Exception("La habitación no tiene tipo de habitación asignado.");

                // Validar huésped
                var huesped = await _context.Huespedes
                    .FirstOrDefaultAsync(h => h.Id == dto.HuespedId);

                if (huesped == null)
                    throw new Exception("El huésped no existe.");

                // Validar fechas
                if (dto.FechaSalida <= dto.FechaEntrada)
                    throw new Exception("La fecha de salida debe ser posterior a la fecha de entrada.");

                // Validar disponibilidad
                var hayConflicto = await _context.Reservas
                    .AnyAsync(r => r.HabitacionId == dto.HabitacionId &&
                                  r.Estado != "cancelada" &&
                                  ((dto.FechaEntrada >= r.FechaEntrada && dto.FechaEntrada < r.FechaSalida) ||
                                   (dto.FechaSalida > r.FechaEntrada && dto.FechaSalida <= r.FechaSalida) ||
                                   (dto.FechaEntrada <= r.FechaEntrada && dto.FechaSalida >= r.FechaSalida)));

                if (hayConflicto)
                    throw new Exception("La habitación no está disponible en las fechas seleccionadas.");

                // Calcular número de noches
                int numeroNoches = (dto.FechaSalida.Date - dto.FechaEntrada.Date).Days;

                if (numeroNoches <= 0)
                    throw new Exception("La reserva debe ser de al menos una noche.");

                // Calcular precio total (itera por cada noche para considerar múltiples temporadas)
                decimal precioTotal = 0;
                decimal precioPrimeraNoche = 0;

                for (int i = 0; i < numeroNoches; i++)
                {
                    DateTime fechaNoche = dto.FechaEntrada.AddDays(i);
                    decimal precioNoche = await _precioService.ObtenerPrecioHabitacionAsync(
                        dto.HabitacionId, 
                        fechaNoche
                    );
                    
                    if (i == 0)
                        precioPrimeraNoche = precioNoche;
                    
                    precioTotal += precioNoche;
                }

                // Crear reserva (NumeroNoches se calculará automáticamente en la BD)
                var reserva = new Reserva
                {
                    HabitacionId = dto.HabitacionId,
                    HuespedId = dto.HuespedId,
                    FechaEntrada = dto.FechaEntrada,
                    FechaSalida = dto.FechaSalida,
                    NumeroHuespedes = dto.NumeroHuespedes, // Ahora ambos son int
                    Estado = "pendiente",
                    PrecioPorNoche = precioPrimeraNoche,
                    PrecioTotal = precioTotal,
                    Observaciones = dto.Observaciones,
                    CreadoPor = usuarioId,
                    CreadoEn = DateTime.UtcNow,
                    ActualizadoEn = DateTime.UtcNow
                };

                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                // Recargar para obtener NumeroNoches y otros valores calculados
                await _context.Entry(reserva).ReloadAsync();

                return await ObtenerReservaPorIdAsync(reserva.Id) 
                    ?? throw new Exception("Error al crear la reserva.");
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"Error de base de datos al crear reserva: {innerMessage}", dbEx);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error al crear reserva: {innerMessage}", ex);
            }
        }

        public async Task<ReservaResponseDto?> ObtenerReservaPorIdAsync(int id)
        {
            return await _context.Reservas
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(r => r.Huesped)
                .Where(r => r.Id == id)
                .Select(r => new ReservaResponseDto
                {
                    Id = r.Id,
                    HabitacionId = r.HabitacionId,
                    HuespedId = r.HuespedId,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,
                    NumeroNoches = r.NumeroNoches,
                    NumeroHuespedes = r.NumeroHuespedes,
                    Estado = r.Estado,
                    PrecioPorNoche = r.PrecioPorNoche,
                    PrecioTotal = r.PrecioTotal,
                    Observaciones = r.Observaciones,
                    CreadoEn = r.CreadoEn,
                    NombreHabitacion = (r.Habitacion != null && r.Habitacion.TipoHabitacion != null) 
                        ? r.Habitacion.NumeroHabitacion + " - " + r.Habitacion.TipoHabitacion.Nombre 
                        : r.Habitacion.NumeroHabitacion ?? "N/A",
                    NombreHuesped = r.Huesped != null 
                        ? (r.Huesped.Nombre ?? "") + " " + (r.Huesped.Apellido ?? "") 
                        : "N/A"
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReservaResponseDto>> ObtenerReservasAsync()
        {
            return await _context.Reservas
                .Include(r => r.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(r => r.Huesped)
                .Select(r => new ReservaResponseDto
                {
                    Id = r.Id,
                    HabitacionId = r.HabitacionId,
                    HuespedId = r.HuespedId,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,
                    NumeroNoches = r.NumeroNoches,
                    NumeroHuespedes = r.NumeroHuespedes,
                    Estado = r.Estado,
                    PrecioPorNoche = r.PrecioPorNoche,
                    PrecioTotal = r.PrecioTotal,
                    Observaciones = r.Observaciones,
                    CreadoEn = r.CreadoEn,
                    NombreHabitacion = (r.Habitacion != null && r.Habitacion.TipoHabitacion != null) 
                        ? r.Habitacion.NumeroHabitacion + " - " + r.Habitacion.TipoHabitacion.Nombre 
                        : r.Habitacion.NumeroHabitacion ?? "N/A",
                    NombreHuesped = r.Huesped != null 
                        ? (r.Huesped.Nombre ?? "") + " " + (r.Huesped.Apellido ?? "") 
                        : "N/A"
                })
                .OrderByDescending(r => r.CreadoEn)
                .ToListAsync();
        }

        public async Task<bool> CancelarReservaAsync(int reservaId, int usuarioId)
        {
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null)
                return false;

            if (reserva.Estado == "cancelada")
                throw new Exception("La reserva ya está cancelada.");

            reserva.Estado = "cancelada";
            reserva.CanceladoPor = usuarioId;
            reserva.FechaCancelacion = DateTime.UtcNow;
            reserva.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}