using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales;

public class Usuarios
{
    [JsonIgnore]
    public int UsuarioID { get; set; }
    public string Identificacion { get; set; }
    public string NombreUsuario { get; set; }
    public string Password { get; set; }
    public char Estado { get; set; }
    public int RolId { get; set; }

}
