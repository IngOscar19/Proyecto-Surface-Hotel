using Hotel.Models;
using Hotel.Services;
using Hotel.Validators;
using ProjectHotel.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly RegistroRequestValidator _registroValidator;
        private readonly LoginRequestValidator _loginValidator;
        private readonly ActualizarUsuarioValidator _actualizarValidator;

        public UsuariosController(
            IUsuarioService usuarioService,
            RegistroRequestValidator registroValidator,
            LoginRequestValidator loginValidator,
            ActualizarUsuarioValidator actualizarValidator)
        {
            _usuarioService = usuarioService;
            _registroValidator = registroValidator;
            _loginValidator = loginValidator;
            _actualizarValidator = actualizarValidator;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
        {
            // Validar con FluentValidation
            var validationResult = await _registroValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    errores = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var usuario = new Usuario
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Email = request.Email,
                    PasswordHash = request.Password, 
                    Rol = request.Rol.ToLower(),
                    Activo = true
                };

                await _usuarioService.CrearUsuarioAsync(usuario);
                return Ok(new { mensaje = "Usuario registrado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Validar con FluentValidation
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    errores = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var token = await _usuarioService.LoginAsync(request.Email, request.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var usuario = await _usuarioService.ObtenerPorIdAsync(userId);
                
                if (usuario == null) 
                    return NotFound(new { mensaje = "Usuario no encontrado" });

                // No devolver el password hash
                return Ok(new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    email = usuario.Email,
                    rol = usuario.Rol,
                    activo = usuario.Activo,
                    creadoEn = usuario.CreadoEn
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] RegistroRequest request)
        {
            // Validar con ActualizarUsuarioValidator (permite password opcional)
            var validationResult = await _actualizarValidator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    errores = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                // Verificar que el usuario solo pueda actualizar su propio perfil (o ser admin)
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (currentUserId != id && userRole != "admin")
                    return Forbid();

                var usuarioUpdate = new Usuario
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Email = request.Email,
                    PasswordHash = request.Password
                };

                var resultado = await _usuarioService.ActualizarUsuarioAsync(id, usuarioUpdate);
                
                if (resultado == null)
                    return NotFound(new { mensaje = "Usuario no encontrado" });

                return Ok(new { mensaje = "Usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Borrar(int id)
        {
            try
            {
                var resultado = await _usuarioService.BorrarUsuarioAsync(id);
                
                if (!resultado)
                    return NotFound(new { mensaje = "Usuario no encontrado" });

                return Ok(new { mensaje = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}