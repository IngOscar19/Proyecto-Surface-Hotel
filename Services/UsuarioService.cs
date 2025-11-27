using Hotel.Data;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Hotel.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HotelDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioService(HotelDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Crear usuario
        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                throw new Exception("El email ya está registrado");

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            usuario.CreadoEn = DateTime.UtcNow;
            usuario.ActualizadoEn = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // Login y generar JWT
        public async Task<string> LoginAsync(string email, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                throw new Exception("Email o contraseña incorrectos");

            // CORREGIDO: Usar "Jwt:Key" en lugar de "JwtKey"
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer,
                Audience = issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Actualizar usuario
        public async Task<Usuario?> ActualizarUsuarioAsync(int id, Usuario usuarioUpdate)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return null;

            usuario.Nombre = usuarioUpdate.Nombre;
            usuario.Apellido = usuarioUpdate.Apellido;
            usuario.Email = usuarioUpdate.Email;
            if (!string.IsNullOrEmpty(usuarioUpdate.PasswordHash))
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuarioUpdate.PasswordHash);

            usuario.ActualizadoEn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return usuario;
        }

        // Borrar usuario
        public async Task<bool> BorrarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener usuario por Id
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
    }
}