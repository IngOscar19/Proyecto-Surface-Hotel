using Hotel.Models;

namespace Hotel.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> CrearUsuarioAsync(Usuario usuario);
        Task<string> LoginAsync(string email, string password);
        Task<Usuario?> ActualizarUsuarioAsync(int id, Usuario usuarioUpdate);
        Task<bool> BorrarUsuarioAsync(int id);
        Task<Usuario?> ObtenerPorIdAsync(int id);
    }
}
