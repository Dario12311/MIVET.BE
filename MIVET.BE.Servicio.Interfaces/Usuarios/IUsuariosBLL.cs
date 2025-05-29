using Microsoft.Extensions.Configuration;
using MIVET.BE.Transversales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public interface IUsuariosBLL
{
    Task<IEnumerable<UsuarioDTO>> GetUsuarioAsync();
    Task<IEnumerable<UsuarioDTO>> GetUsuarioByIdAsync(string id);
    Task<Usuarios> CreateUsuarioAsync(Usuarios usuario);
    Task<Usuarios> UpdateUsuarioAsync(Usuarios usuario);
    Task DeleteUsuarioAsync(string id);
    Task<string> LoginAsync(string identificacion, string password, IConfiguration config);
    Task AgregarRolesUsuarioAsync(Usuarios usuarioBase, List<int> rolesAAgregar);
    Task<bool> ResetPasswordAsync(ResetPassword dto);

}
