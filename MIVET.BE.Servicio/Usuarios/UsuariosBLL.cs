using MedicalWeb.BE.Transversales.Encriptacion;
using Microsoft.Extensions.Configuration;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio;

public class UsuariosBLL: IUsuariosBLL
{
    private readonly IUsuariosDAL _usuarioDAL;
    private readonly IConfiguration _configuration;

    public UsuariosBLL(IUsuariosDAL usuarioDAL, IConfiguration configuration)
    {
        _usuarioDAL = usuarioDAL;
        _configuration = configuration;
    }

    public async Task<IEnumerable<UsuarioDTO>> GetUsuarioByIdAsync(string id)
    {
        var usuarios = await _usuarioDAL.GetUsuarioByIdAsync(id);

        var usuariosDTO = usuarios.Select(u => new UsuarioDTO
        {
            UsuarioID = u.UsuarioID,
            Identificacion = u.Identificacion,
            NombreUsuario = u.NombreUsuario,
            Password = u.Password,
            Estado = u.Estado,
            RolId = Rol.GetRolById(u.RolId)?.Nombre
        }).ToList();

        return usuariosDTO;
    }

    public async Task<IEnumerable<UsuarioDTO>> GetUsuarioAsync()
    {
        return await _usuarioDAL.GetUsuarioAsync();
    }

    public async Task<Usuarios> CreateUsuarioAsync(Usuarios usuario)
    {
        return await _usuarioDAL.CreateUsuarioAsync(usuario);
    }

    public async Task DeleteUsuarioAsync(string id)
    {
        await _usuarioDAL.DeleteUsuarioAsync(id);
    }

    public async Task<Usuarios> UpdateUsuarioAsync(Usuarios usuario)
    {
        return await _usuarioDAL.UpdateUsuarioAsync(usuario);
    }

    public async Task<string> LoginAsync(string nombreUsuario, string password, IConfiguration config)
    {

        string passwordEncriptada = Encrypt.EncriptarContrasena(password);

        var usuarios = await _usuarioDAL.GetUsuarioByCredentialsAsync(nombreUsuario, passwordEncriptada);

        if (usuarios == null || !usuarios.Any())
        {
            throw new UnauthorizedAccessException("Credenciales incorrectas.");
        }

        if (usuarios.Any(u => u.Estado != 'A'))
        {
            throw new UnauthorizedAccessException("El usuario no está activo.");
        }

        var usuarioBase = usuarios.First();
        var rolesCombinados = string.Join(",", usuarios
            .Select(u => Rol.GetRolById(u.RolId)?.Nombre)
            .Where(nombre => !string.IsNullOrEmpty(nombre)));

        var usuarioDTO = new UsuarioDTO
        {
            Identificacion = usuarioBase.Identificacion,
            NombreUsuario = usuarioBase.NombreUsuario,
            RolId = rolesCombinados
        };

        return JwtConfiguration.GetToken(usuarioDTO, config);
    }

    public async Task<bool> ActualizarRolesUsuarioAsync(string identificacion, List<int> nuevosRoles)
    {
        // Obtener todos los usuarios con la misma Identificación
        var usuariosExistentes = await _usuarioDAL.ObtenerUsuariosPorIdentificacionAsync(identificacion);

        if (!usuariosExistentes.Any())
            return false;

        var rolesActuales = usuariosExistentes.Select(u => u.RolId).ToList();

        // Determinar roles a eliminar (los que ya no están en la nueva lista)
        var rolesAEliminar = rolesActuales.Except(nuevosRoles).ToList();

        // Determinar roles a agregar (los nuevos que antes no estaban)
        var rolesAAgregar = nuevosRoles.Except(rolesActuales).ToList();

        // Eliminar solo los registros con roles obsoletos
        if (rolesAEliminar.Any())
            await _usuarioDAL.EliminarRolesUsuarioAsync(identificacion, rolesAEliminar);

        // Agregar nuevos roles sin perder la información del usuario
        if (rolesAAgregar.Any())
        {
            var usuarioBase = usuariosExistentes.First(); // Tomamos un usuario de referencia para mantener los datos
            await _usuarioDAL.AgregarRolesUsuarioAsync(usuarioBase, rolesAAgregar);
        }

        return true; // Operación exitosa
    }

    public async Task<bool> ResetPasswordAsync(ResetPassword dto)
    {
        return await _usuarioDAL.ResetPasswordAsync(dto.Identificacion, dto.NuevaPassword);
    }
}
