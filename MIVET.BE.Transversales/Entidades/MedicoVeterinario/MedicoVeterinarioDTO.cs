using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class MedicoVeterinarioDTO
{
    public int Id { get; set; } = 0;
    public string Nombre { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public int EstadoCivil { get; set; } = 0;
    public int TipoDocumentoId { get; set; } = 0;
    public string Especialidad { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public string UniversidadGraduacion { get; set; } = string.Empty;
    public DateTime AñoGraduacion { get; set; } = DateTime.Now;
    public DateTime FechaNacimiento { get; set; } = DateTime.Now;
    public string nacionalidad { get; set; } = string.Empty;
    public string genero { get; set; } = string.Empty;
    public string ciudad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
