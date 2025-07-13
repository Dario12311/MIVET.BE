using System.Text.Json.Serialization;

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

    // ⬅️ Agregar campos adicionales
    public string PrimerNombre { get; set; }
    public string SegundoNombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string CorreoElectronico { get; set; }
    public string Telefono { get; set; }
    public string Celular { get; set; }
    public string Direccion { get; set; }
    public string Ciudad { get; set; }
}
