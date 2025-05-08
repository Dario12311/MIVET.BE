using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class MedicoVeterinario
{

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string NumeroDocumento { get; set; }
    public int EstadoCivil { get; set; }
    public int TipoDocumentoId { get; set; }
    public string Especialidad { get; set; }
    public string Telefono { get; set; }
    public string CorreoElectronico { get; set; }
    public string Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string UniversidadGraduacion { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public DateTime AñoGraduacion { get; set; }
    public string genero { get; set; }
    public string Estado { get; set; }
    public string nacionalidad { get; set; }
    public string ciudad { get; set; }

    [JsonIgnore]
    public TipoDocumento TipoDocumento { get; set; }
    [JsonIgnore]
    public MIVET.BE.Transversales.Entidades.MaritalStatus.MaritalStatus MaritalStatus { get; set; }

}
