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

public class UsuariosBLL : IUsuariosBLL
{
    private readonly IUsuariosDAL _usuarioDAL;
    private readonly IConfiguration _configuration;
    private readonly IPersonaClienteDAL _clienteDAL;

    public UsuariosBLL(IUsuariosDAL usuarioDAL, IConfiguration configuration, IPersonaClienteDAL clienteDAL)
    {
        _usuarioDAL = usuarioDAL;
        _configuration = configuration;
        _clienteDAL = clienteDAL;
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

        // ⬅️ Obtener información adicional del cliente
        var clienteInfo = await _clienteDAL.GetByIdAsync(usuarioBase.Identificacion);

        var usuarioDTO = new UsuarioDTO
        {
            Identificacion = usuarioBase.Identificacion,
            NombreUsuario = usuarioBase.Identificacion,
            RolId = rolesCombinados,

            // ⬅️ Agregar información del cliente si existe
            PrimerNombre = clienteInfo?.PrimerNombre ?? "",
            SegundoNombre = clienteInfo?.SegundoNombre ?? "",
            PrimerApellido = clienteInfo?.PrimerApellido ?? "",
            SegundoApellido = clienteInfo?.SegundoApellido ?? "",
            CorreoElectronico = clienteInfo?.CorreoElectronico ?? "",
            Telefono = clienteInfo?.Telefono ?? "",
            Celular = clienteInfo?.Celular ?? "",
            Direccion = clienteInfo?.Direccion ?? "",
            Ciudad = clienteInfo?.Ciudad ?? ""
        };

        return JwtConfiguration.GetToken(usuarioDTO, config);
    }

    public async Task<bool> ResetPasswordAsync(ResetPassword dto)
    {
        return await _usuarioDAL.ResetPasswordAsync(dto.Identificacion, dto.NuevaPassword);
    }

    public async Task AgregarRolesUsuarioAsync(Usuarios usuarioBase, List<int> rolesAAgregar)
    {
        await _usuarioDAL.AgregarRolesUsuarioAsync(usuarioBase, rolesAAgregar);
    }
}
