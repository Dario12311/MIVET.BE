using MedicalWeb.BE.Transversales.Encriptacion;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Repositorio;

public class UsuariosDAL: IUsuariosDAL
{
    private readonly MIVETDbContext _context;
    public UsuariosDAL(MIVETDbContext context)
    {
        _context = context;
    }

    public async Task<Usuarios> CreateUsuarioAsync(Usuarios usuario)
    {
        usuario.Password = Encrypt.EncriptarContrasena(usuario.Password);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task DeleteUsuarioAsync(string identificacion)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Identificacion == identificacion);

        if (usuario != null)
        {
            usuario.Estado = 'I';
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<UsuarioDTO>> GetUsuarioAsync()
    {
        // Cargar los roles en memoria solo una vez
        var rolesDict = Rol.GetAll().ToDictionary(r => r.Id, r => r.Nombre);

        var usuariosDTO = await (from u in _context.Usuarios
                                 group u by u.Identificacion into g
                                 select new UsuarioDTO
                                 {
                                     UsuarioID = g.First().UsuarioID, // Se toma solo un valor representativo
                                     Identificacion = g.Key,
                                     NombreUsuario = g.First().NombreUsuario,
                                     Password = g.First().Password,
                                     Estado = g.First().Estado,

                                     // Concatenar roles directamente en SQL
                                     RolId = string.Join(", ", g.Select(u => rolesDict.ContainsKey(u.RolId) ? rolesDict[u.RolId] : "Desconocido")
                                                                .Distinct()),

                                     // Obtener correos con LEFT JOIN en SQL y evitar múltiples consultas a la DB
                                     Correos = string.Join(", ",
                                         (from p in _context.PersonaCliente
                                          where p.NumeroDocumento == g.Key
                                          select p.CorreoElectronico)
                                         .Union(
                                             from m in _context.MedicoVeterinario
                                             where m.NumeroDocumento == g.Key
                                             select m.CorreoElectronico
                                         )
                                     )
                                 }).ToListAsync();

        return usuariosDTO;
    }

    public async Task<IEnumerable<Usuarios>> GetUsuarioByIdAsync(string id)
    {
        var usuarios = await _context.Usuarios
            .Where(ur => ur.Identificacion == id)
            .ToListAsync();

        return usuarios;
    }

    public async Task<Usuarios> UpdateUsuarioAsync(Usuarios usuario)
    {
        var usuarioExistente = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Identificacion == usuario.Identificacion);

        if (usuarioExistente == null)
        {
            throw new KeyNotFoundException("El usuario no existe.");
        }

        // Mantener el RolId del usuario existente
        usuario.UsuarioID = usuarioExistente.UsuarioID;

        // Actualizar los campos con los nuevos valores
        usuarioExistente.NombreUsuario = usuario.NombreUsuario;
        usuarioExistente.Password = Encrypt.EncriptarContrasena(usuario.Password);
        usuarioExistente.Estado = usuario.Estado;

        // Evitar la modificación de RolId
        _context.Entry(usuarioExistente).Property(u => u.RolId).IsModified = false;

        await _context.SaveChangesAsync();

        return new Usuarios
        {
            UsuarioID = usuarioExistente.UsuarioID,
            Identificacion = usuarioExistente.Identificacion,
            NombreUsuario = usuarioExistente.NombreUsuario,
            Password = usuario.Password, // Retornamos la nueva contraseña en texto plano si es necesario
            Estado = usuarioExistente.Estado
        };
    }


    public async Task<IEnumerable<Usuarios>> GetUsuarioByCredentialsAsync(string nombreUsuario, string passwordEncriptada)
    {
        return await _context.Usuarios
            .Where(u => u.Identificacion == nombreUsuario &&
                        u.Password == passwordEncriptada &&
                        u.Estado == 'A')
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuarios>> ObtenerUsuariosPorIdentificacionAsync(string identificacion)
    {
        return await _context.Usuarios
        .Where(u => u.Identificacion == identificacion)
        .ToListAsync();
    }

    public async Task EliminarRolesUsuarioAsync(string identificacion, List<int> rolesAEliminar)
    {
        var usuarios = await _context.Usuarios
            .Where(u => u.Identificacion == identificacion && rolesAEliminar.Contains(u.RolId))
            .ToListAsync();

        _context.Usuarios.RemoveRange(usuarios);
        await _context.SaveChangesAsync();
    }

    public async Task AgregarRolesUsuarioAsync(Usuarios usuarioBase, List<int> rolesAAgregar)
    {
        // Obtener la contraseña encriptada actual del usuario
        var usuarioExistente = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Identificacion == usuarioBase.Identificacion);

        if (usuarioExistente == null)
            throw new Exception("El usuario base no existe.");

        string passwordEncriptada = usuarioExistente.Password;

        foreach (var rolId in rolesAAgregar)
        {
            // Verifica que NO exista ya ese rol para ese usuario
            var existe = await _context.Usuarios
                .AnyAsync(u => u.Identificacion == usuarioBase.Identificacion && u.RolId == rolId);

            if (!existe)
            {
                var nuevoUsuario = new Usuarios
                {
                    Identificacion = usuarioBase.Identificacion,
                    NombreUsuario = usuarioBase.NombreUsuario,
                    Password = passwordEncriptada, // Usar la contraseña encriptada existente
                    Estado = usuarioBase.Estado,
                    RolId = rolId
                };

                await _context.Usuarios.AddAsync(nuevoUsuario);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ResetPasswordAsync(string identificacion, string nuevaPassword)
    {
        var usuarios = await _context.Usuarios
            .Where(u => u.Identificacion == identificacion)
            .ToListAsync(); // Obtiene todos los usuarios con esa identificación

        if (!usuarios.Any()) // Verifica si la lista está vacía
        {
            throw new KeyNotFoundException("El usuario no existe.");
        }

        string nuevaPasswordEncriptada = Encrypt.EncriptarContrasena(nuevaPassword);

        foreach (var usuario in usuarios)
        {
            usuario.Password = nuevaPasswordEncriptada;
            _context.Entry(usuario).Property(u => u.Password).IsModified = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
