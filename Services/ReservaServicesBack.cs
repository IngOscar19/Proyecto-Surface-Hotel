using Hotel.Data;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hotel.Services
{
    public class ReservaBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaBackgroundService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(5); 

        public ReservaBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<ReservaBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           

            // Ejecutar inmediatamente al inicio
            await ActualizarEstadosHabitaciones(stoppingToken);

            // Luego ejecutar periódicamente
            using var timer = new PeriodicTimer(_intervalo);

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ActualizarEstadosHabitaciones(stoppingToken);
            }

            _logger.LogInformation("⛔ ReservaBackgroundService detenido");
        }

        private async Task ActualizarEstadosHabitaciones(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

                var ahora = DateTime.UtcNow; 
                var hoy = ahora.Date; 

                _logger.LogInformation("Verificando estados de habitaciones");

                
                
                var reservasActivas = await context.Reservas
                    .Include(r => r.Habitacion)
                    .Where(r => r.Estado == "confirmada" &&
                               r.FechaEntrada.Date <= hoy &&
                               r.FechaSalida.Date > hoy &&
                               r.Habitacion.Estado != "ocupada")
                    .ToListAsync(stoppingToken);

                foreach (var reserva in reservasActivas)
                {
                    _logger.LogInformation($"Habitación {reserva.Habitacion.NumeroHabitacion} → OCUPADA (Reserva #{reserva.Id})");
                    reserva.Habitacion.Estado = "ocupada";
                }


                var reservasFinalizadas = await context.Reservas
                    .Include(r => r.Habitacion)
                    .Where(r => (r.Estado == "confirmada" || r.Estado == "completada") &&
                               r.FechaSalida <= ahora && 
                               r.Habitacion.Estado == "ocupada")
                    .ToListAsync(stoppingToken);

                foreach (var reserva in reservasFinalizadas)
                {
                    _logger.LogInformation($"Habitación {reserva.Habitacion.NumeroHabitacion} → LIMPIEZA (Checkout de Reserva #{reserva.Id})");
                    reserva.Habitacion.Estado = "limpieza";
                    
                    
                    if (reserva.Estado != "completada")
                    {
                        reserva.Estado = "completada";
                        reserva.ActualizadoEn = ahora;
                    }
                }

                
                
                var habitacionesLimpieza = await context.Habitaciones
                    .Where(h => h.Estado == "limpieza")
                    .ToListAsync(stoppingToken);

                foreach (var habitacion in habitacionesLimpieza)
                {
                    
                    var tieneReservaProxima = await context.Reservas
                        .AnyAsync(r => r.HabitacionId == habitacion.Id &&
                                      r.Estado == "confirmada" &&
                                      r.FechaEntrada.Date <= hoy.AddDays(1) &&
                                      r.FechaEntrada.Date > hoy,
                                      stoppingToken);

                    if (!tieneReservaProxima)
                    {
                        
                        var horasDesdeActualizacion = (ahora - habitacion.ActualizadoEn).TotalHours;
                        
                        if (horasDesdeActualizacion >= 2)
                        {
                            _logger.LogInformation($"Habitación {habitacion.NumeroHabitacion} → DISPONIBLE (Limpieza completada)");
                            habitacion.Estado = "disponible";
                            habitacion.ActualizadoEn = ahora;
                        }
                    }
                }

                
                var reservasPendientesVencidas = await context.Reservas
                    .Where(r => r.Estado == "pendiente" &&
                               r.FechaEntrada.Date < hoy.AddDays(-1))
                    .ToListAsync(stoppingToken);

                foreach (var reserva in reservasPendientesVencidas)
                {
                    _logger.LogWarning($"Reserva #{reserva.Id} CANCELADA automáticamente (No confirmada a tiempo)");
                    reserva.Estado = "cancelada";
                    reserva.FechaCancelacion = ahora;
                    reserva.Observaciones += " [Cancelada automáticamente - No confirmada]";
                    reserva.ActualizadoEn = ahora;
                }

                
                var cambios = await context.SaveChangesAsync(stoppingToken);

                if (cambios > 0)
                {
                    _logger.LogInformation($"Se actualizaron {cambios} registros");
                }
                else
                {
                    _logger.LogInformation("No hay cambios en los estados");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estados de habitaciones: {Message}", ex.Message);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deteniendo ReservaBackgroundService...");
            return base.StopAsync(cancellationToken);
        }
    }
}