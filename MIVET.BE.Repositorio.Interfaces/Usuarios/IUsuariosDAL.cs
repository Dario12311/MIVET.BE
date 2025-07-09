using MIVET.BE.Transversales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio.Interfaces;
public interface IUsuariosDAL
{
    Task<IEnumerable<UsuarioDTO>> GetUsuarioAsync();
    Task<IEnumerable<Usuarios>> GetUsuarioByIdAsync(string id);
    Task<Usuarios> CreateUsuarioAsync(Usuarios usuario);
    Task<Usuarios> UpdateUsuarioAsync(Usuarios usuario);
    Task DeleteUsuarioAsync(string id);
    Task<IEnumerable<Usuarios>> GetUsuarioByCredentialsAsync(string nombreUsuario, string passwordEncriptada);
    Task<IEnumerable<Usuarios>> ObtenerUsuariosPorIdentificacionAsync(string identificacion);
    Task EliminarRolesUsuarioAsync(string identificacion, List<int> rolesAEliminar);
    Task AgregarRolesUsuarioAsync(Usuarios usuarioBase, List<int> rolesAAgregar);
    Task<bool> ResetPasswordAsync(string identificacion, string nuevaPassword);
}
