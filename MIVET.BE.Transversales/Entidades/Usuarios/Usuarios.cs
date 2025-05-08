using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class Usuarios
{
    public int UsuarioId { get; set; }
    public string Identificacion { get; set; } 
    public string NombreUsuario { get; set; }
    public string Password { get; set; }
    public int RolId { get; set; }
    public string NumeroDocumento { get; set; }
    public char Estado { get; set; }

}
