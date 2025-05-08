using MedicalWeb.BE.Transversales.Encriptacion;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.Entidades;
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

    public async Task<Usuarios> CreateUsuarioAsync(Usuarios usuarios)
    {
        usuarios.Password = Encrypt.EncriptarContrasena(usuarios.Password);
        _context.Usuarios.Add(usuarios);
        await _context.SaveChangesAsync();
        return usuarios;
    }
}
