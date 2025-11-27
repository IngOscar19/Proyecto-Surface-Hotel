using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hotel.Data;
using Hotel.Models;
using Hotel.Services.Interfaces;

namespace Hotel.Services
{
    public class TemporadaHabitacionPrecioService : ITemporadaHabitacionPrecioService
    {
        private readonly HotelDbContext _context;

        public TemporadaHabitacionPrecioService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TemporadaHabitacionPrecio>> ObtenerTodosAsync()
        {
            return await _context.Set<TemporadaHabitacionPrecio>()
                .Include(thp => thp.Temporada)
                .Include(thp => thp.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<TemporadaHabitacionPrecio>> ObtenerPorTemporadaAsync(int temporadaId)
        {
            return await _context.Set<TemporadaHabitacionPrecio>()
                .Where(thp => thp.TemporadaId == temporadaId)
                .Include(thp => thp.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .Include(thp => thp.Temporada)
                .OrderBy(thp => thp.Habitacion.NumeroHabitacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<TemporadaHabitacionPrecio>> ObtenerPorHabitacionAsync(int habitacionId)
        {
            return await _context.Set<TemporadaHabitacionPrecio>()
                .Where(thp => thp.HabitacionId == habitacionId)
                .Include(thp => thp.Temporada)
                .Include(thp => thp.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .OrderBy(thp => thp.Temporada.FechaInicio)
                .ToListAsync();
        }

        public async Task<TemporadaHabitacionPrecio?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<TemporadaHabitacionPrecio>()
                .Include(thp => thp.Temporada)
                .Include(thp => thp.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(thp => thp.Id == id);
        }

        public async Task<TemporadaHabitacionPrecio?> ObtenerPorTemporadaYHabitacionAsync(int temporadaId, int habitacionId)
        {
            return await _context.Set<TemporadaHabitacionPrecio>()
                .Include(thp => thp.Temporada)
                .Include(thp => thp.Habitacion)
                    .ThenInclude(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(thp => 
                    thp.TemporadaId == temporadaId && 
                    thp.HabitacionId == habitacionId);
        }

        public async Task<decimal> ObtenerPrecioHabitacionAsync(int habitacionId, DateTime fecha)
        {
            // 1. Cargar habitación + tipo
            var habitacion = await _context.Habitaciones
                .Include(h => h.TipoHabitacion)
                .FirstOrDefaultAsync(h => h.Id == habitacionId);

            if (habitacion == null)
                throw new InvalidOperationException("Habitación no encontrada");

            // 2. Detectar temporada activa
            var temporada = await _context.Set<TemporadaPrecio>()
                .Where(t => t.Activo &&
                    fecha.Date >= t.FechaInicio.Date &&
                    fecha.Date <= t.FechaFin.Date)
                .FirstOrDefaultAsync();


            // Si NO hay temporada → base * tipo
            if (temporada == null)
            {
                return habitacion.PrecioBase * habitacion.TipoHabitacion.FactorTipo;
            }

            // 3. Buscar override
            var overridePrecio = await _context.TemporadaHabitacionPrecios
                .Where(thp => thp.TemporadaId == temporada.Id &&
                              thp.HabitacionId == habitacionId)
                .FirstOrDefaultAsync();

            if (overridePrecio != null)
            {
                return overridePrecio.PrecioOverride;
            }

            // 4. Calcular precio normal (base * temporada * tipo)
            decimal precio = habitacion.PrecioBase;

            precio *= temporada.FactorMultiplicador;
            precio *= habitacion.TipoHabitacion.FactorTipo;

            return precio;
        }

        public async Task<TemporadaHabitacionPrecio> CrearAsync(TemporadaHabitacionPrecio temporadaHabitacionPrecio)
        {
            if (await ExistePrecioAsync(temporadaHabitacionPrecio.TemporadaId, temporadaHabitacionPrecio.HabitacionId))
            {
                throw new InvalidOperationException("Ya existe un precio definido para esta habitación en esta temporada");
            }

            bool temporadaExiste = await _context.Set<TemporadaPrecio>()
                .AnyAsync(t => t.Id == temporadaHabitacionPrecio.TemporadaId);


            bool habitacionExiste = await _context.Set<Habitacion>()
                .AnyAsync(h => h.Id == temporadaHabitacionPrecio.HabitacionId);

            if (!temporadaExiste || !habitacionExiste)
            {
                throw new InvalidOperationException("La temporada o habitación especificada no existe");
            }

            temporadaHabitacionPrecio.CreadoEn = DateTime.UtcNow;

            _context.Set<TemporadaHabitacionPrecio>().Add(temporadaHabitacionPrecio);
            await _context.SaveChangesAsync();

            return temporadaHabitacionPrecio;
        }

        public async Task<IEnumerable<TemporadaHabitacionPrecio>> CrearMultiplesAsync(int temporadaId, List<TemporadaHabitacionPrecio> precios)
        {
            bool temporadaExiste = await _context.Set<TemporadaPrecio>()
                .AnyAsync(t => t.Id == temporadaId);

            if (!temporadaExiste)
            {
                throw new InvalidOperationException("La temporada especificada no existe");
            }

            var habitacionIds = precios.Select(p => p.HabitacionId).ToList();

            if (habitacionIds.Count != habitacionIds.Distinct().Count())
            {
                throw new InvalidOperationException("No se pueden crear múltiples precios para la misma habitación");
            }

            foreach (var precio in precios)
            {
                if (await ExistePrecioAsync(temporadaId, precio.HabitacionId))
                {
                    throw new InvalidOperationException(
                        $"Ya existe un precio para la habitación {precio.HabitacionId} en esta temporada");
                }

                precio.TemporadaId = temporadaId;
                precio.CreadoEn = DateTime.UtcNow;
            }

            _context.Set<TemporadaHabitacionPrecio>().AddRange(precios);
            await _context.SaveChangesAsync();

            return precios;
        }

        public async Task<TemporadaHabitacionPrecio?> ActualizarAsync(int id, TemporadaHabitacionPrecio temporadaHabitacionPrecio)
        {
            var precioExistente = await _context.Set<TemporadaHabitacionPrecio>()
                .FindAsync(id);

            if (precioExistente == null)
            {
                return null;
            }

            if (await ExistePrecioAsync(temporadaHabitacionPrecio.TemporadaId, temporadaHabitacionPrecio.HabitacionId, id))
            {
                throw new InvalidOperationException("Ya existe otro precio definido para esta habitación en esta temporada");
            }

            precioExistente.TemporadaId = temporadaHabitacionPrecio.TemporadaId;
            precioExistente.HabitacionId = temporadaHabitacionPrecio.HabitacionId;
            precioExistente.PrecioOverride = temporadaHabitacionPrecio.PrecioOverride;

            await _context.SaveChangesAsync();

            return precioExistente;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var precio = await _context.Set<TemporadaHabitacionPrecio>()
                .FindAsync(id);

            if (precio == null)
            {
                return false;
            }

            _context.Set<TemporadaHabitacionPrecio>().Remove(precio);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarPorTemporadaAsync(int temporadaId)
        {
            var precios = await _context.Set<TemporadaHabitacionPrecio>()
                .Where(thp => thp.TemporadaId == temporadaId)
                .ToListAsync();

            if (!precios.Any())
            {
                return false;
            }

            _context.Set<TemporadaHabitacionPrecio>().RemoveRange(precios);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistePrecioAsync(int temporadaId, int habitacionId, int? idExcluir = null)
        {
            var query = _context.Set<TemporadaHabitacionPrecio>()
                .Where(thp => thp.TemporadaId == temporadaId &&
                             thp.HabitacionId == habitacionId);

            if (idExcluir.HasValue)
            {
                query = query.Where(thp => thp.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
    }
}
