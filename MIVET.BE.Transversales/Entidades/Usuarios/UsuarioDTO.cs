using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales;

public class UsuarioDTO
{
    [JsonIgnore]
    public int UsuarioID { get; set; }
    public string Identificacion { get; set; }
    public string NombreUsuario { get; set; }
    public string Password { get; set; }
    public char Estado { get; set; }
    public string RolId { get; set; }
    public string Correos { get; set; }
}
