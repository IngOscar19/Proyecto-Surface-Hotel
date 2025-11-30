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
    public class TipoHabitacionService : ITipoHabitacionService
    {
        private readonly HotelDbContext _context;

        public TipoHabitacionService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoHabitacion>> ObtenerTodosAsync()
        {
            return await _context.TiposHabitacion
                .Include(t => t.Habitaciones)
                .OrderBy(t => t.Nombre)
                .ToListAsync();
        }

        public async Task<TipoHabitacion?> ObtenerPorIdAsync(int id)
        {
            return await _context.TiposHabitacion
                .Include(t => t.Habitaciones)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TipoHabitacion> CrearAsync(TipoHabitacion tipoHabitacion)
        {
            // Validar que no exista un tipo con el mismo nombre
            if (await ExisteNombreAsync(tipoHabitacion.Nombre))
            {
                throw new InvalidOperationException($"Ya existe un tipo de habitación con el nombre '{tipoHabitacion.Nombre}'");
            }

            // Validar que el factor sea positivo
            if (tipoHabitacion.FactorTipo <= 0)
            {
                throw new InvalidOperationException("El factor de tipo debe ser mayor a 0");
            }

            tipoHabitacion.CreadoEn = DateTime.UtcNow;

            _context.TiposHabitacion.Add(tipoHabitacion);
            await _context.SaveChangesAsync();

            return tipoHabitacion;
        }

        public async Task<TipoHabitacion?> ActualizarAsync(int id, TipoHabitacion tipoHabitacion)
        {
            var tipoExistente = await _context.TiposHabitacion.FindAsync(id);

            if (tipoExistente == null)
            {
                return null;
            }

            // Validar que no exista otro tipo con el mismo nombre
            if (await ExisteNombreAsync(tipoHabitacion.Nombre, id))
            {
                throw new InvalidOperationException($"Ya existe otro tipo de habitación con el nombre '{tipoHabitacion.Nombre}'");
            }

            // Validar que el factor sea positivo
            if (tipoHabitacion.FactorTipo <= 0)
            {
                throw new InvalidOperationException("El factor de tipo debe ser mayor a 0");
            }

            tipoExistente.Nombre = tipoHabitacion.Nombre;
            tipoExistente.Descripcion = tipoHabitacion.Descripcion;
            tipoExistente.FactorTipo = tipoHabitacion.FactorTipo;

            await _context.SaveChangesAsync();

            return tipoExistente;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var tipo = await _context.TiposHabitacion
                .Include(t => t.Habitaciones)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tipo == null)
            {
                return false;
            }

            // Validar que no tenga habitaciones asociadas
            if (tipo.Habitaciones != null && tipo.Habitaciones.Any())
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar el tipo '{tipo.Nombre}' porque tiene {tipo.Habitaciones.Count} habitación(es) asociada(s)");
            }

            _context.TiposHabitacion.Remove(tipo);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
        {
            var query = _context.TiposHabitacion
                .Where(t => t.Nombre.ToLower() == nombre.ToLower());

            if (idExcluir.HasValue)
            {
                query = query.Where(t => t.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
    }
}