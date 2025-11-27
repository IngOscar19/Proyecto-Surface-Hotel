using Hotel.Data;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using ProjectHotel.DTOs;

namespace Hotel.Services
{
    public class HuespedService : IHuespedService
    {
        private readonly HotelDbContext _context;

        public HuespedService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<Huesped> Crear(CrearHuespedDTO dto)
        {
            var huesped = new Huesped
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Telefono = dto.Telefono,
                NumeroDocumento = dto.NumeroDocumento,
                TipoDocumento = dto.TipoDocumento,
                Nacionalidad = dto.Nacionalidad,
                Direccion = dto.Direccion,
                FechaNacimiento = dto.FechaNacimiento,
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };

            _context.Huespedes.Add(huesped);
            await _context.SaveChangesAsync();

            return huesped;
        }

        public async Task<List<Huesped>> Listar()
        {
            return await _context.Huespedes.ToListAsync();
        }

        public async Task<Huesped?> Obtener(int id)
        {
            return await _context.Huespedes.FindAsync(id);
        }

        public async Task<Huesped?> Actualizar(int id, CrearHuespedDTO dto)
        {
            var huesped = await _context.Huespedes.FindAsync(id);
            if (huesped == null) return null;

            huesped.Nombre = dto.Nombre;
            huesped.Apellido = dto.Apellido;
            huesped.Email = dto.Email;
            huesped.Telefono = dto.Telefono;
            huesped.NumeroDocumento = dto.NumeroDocumento;
            huesped.TipoDocumento = dto.TipoDocumento;
            huesped.Nacionalidad = dto.Nacionalidad;
            huesped.Direccion = dto.Direccion;
            huesped.FechaNacimiento = dto.FechaNacimiento;
            huesped.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return huesped;
        }

        public async Task<bool> Eliminar(int id)
        {
            var huesped = await _context.Huespedes.FindAsync(id);
            if (huesped == null) return false;

            // OJO: aquí no bloqueamos eliminación aunque haya reservas.
            // Eso lo podemos ajustar después si tú quieres.
            _context.Huespedes.Remove(huesped);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
