using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;
using Hotel.Services;

namespace Hotel.Services
{
    public class TemporadaPrecioService : ITemporadaPrecioService
    {
        private readonly HotelDbContext _context;

        public TemporadaPrecioService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TemporadaPrecio>> ObtenerTodosAsync()
        {
            return await _context.Set<TemporadaPrecio>()
                .Include(t => t.HabitacionPrecios)
                .OrderBy(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TemporadaPrecio>> ObtenerActivosAsync()
        {
            return await _context.Set<TemporadaPrecio>()
                .Where(t => t.Activo)
                .Include(t => t.HabitacionPrecios)
                .OrderBy(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<TemporadaPrecio?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<TemporadaPrecio>()
                .Include(t => t.HabitacionPrecios)
                    .ThenInclude(hp => hp.Habitacion)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TemporadaPrecio?> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.Set<TemporadaPrecio>()
                .Where(t => t.Activo && 
                           fecha.Date >= t.FechaInicio.Date && 
                           fecha.Date <= t.FechaFin.Date)
                .Include(t => t.HabitacionPrecios)
                .FirstOrDefaultAsync();
        }

        public async Task<TemporadaPrecio> CrearAsync(TemporadaPrecio temporada)
        {
            // Validar traslape de fechas
            if (await ExisteTraslapeAsync(temporada.FechaInicio, temporada.FechaFin))
            {
                throw new InvalidOperationException("Ya existe una temporada que se traslapa con estas fechas");
            }

            temporada.CreadoEn = DateTime.UtcNow;
            temporada.ActualizadoEn = DateTime.UtcNow;

            _context.Set<TemporadaPrecio>().Add(temporada);
            await _context.SaveChangesAsync();

            return temporada;
        }

        public async Task<TemporadaPrecio?> ActualizarAsync(int id, TemporadaPrecio temporada)
        {
            var temporadaExistente = await _context.Set<TemporadaPrecio>()
                .FindAsync(id);

            if (temporadaExistente == null)
            {
                return null;
            }

            // Validar traslape de fechas (excluyendo la temporada actual)
            if (await ExisteTraslapeAsync(temporada.FechaInicio, temporada.FechaFin, id))
            {
                throw new InvalidOperationException("Ya existe una temporada que se traslapa con estas fechas");
            }

            temporadaExistente.Nombre = temporada.Nombre;
            temporadaExistente.Descripcion = temporada.Descripcion;
            temporadaExistente.FechaInicio = temporada.FechaInicio;
            temporadaExistente.FechaFin = temporada.FechaFin;
            temporadaExistente.FactorMultiplicador = temporada.FactorMultiplicador;
            temporadaExistente.Activo = temporada.Activo;
            temporadaExistente.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return temporadaExistente;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var temporada = await _context.Set<TemporadaPrecio>()
                .Include(t => t.HabitacionPrecios)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (temporada == null)
            {
                return false;
            }

            // Eliminar precios asociados primero
            _context.Set<TemporadaHabitacionPrecio>().RemoveRange(temporada.HabitacionPrecios);
            _context.Set<TemporadaPrecio>().Remove(temporada);
            
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteTraslapeAsync(DateTime fechaInicio, DateTime fechaFin, int? temporadaIdExcluir = null)
        {
            var query = _context.Set<TemporadaPrecio>()
                .Where(t => t.Activo &&
                           ((fechaInicio.Date >= t.FechaInicio.Date && fechaInicio.Date <= t.FechaFin.Date) ||
                            (fechaFin.Date >= t.FechaInicio.Date && fechaFin.Date <= t.FechaFin.Date) ||
                            (fechaInicio.Date <= t.FechaInicio.Date && fechaFin.Date >= t.FechaFin.Date)));

            if (temporadaIdExcluir.HasValue)
            {
                query = query.Where(t => t.Id != temporadaIdExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ActivarDesactivarAsync(int id, bool activo)
        {
            var temporada = await _context.Set<TemporadaPrecio>()
                .FindAsync(id);

            if (temporada == null)
            {
                return false;
            }

            temporada.Activo = activo;
            temporada.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}