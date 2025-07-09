using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class HistoriaClinicaMascota
{
    public int Id { get; set; }
    public int IdMascota { get; set; }
    public string NumeroDocumentoPropietario { get; set; }
    public string NombrePropietario { get; set; }
    public string NombreMascota { get; set; }
    public string Raza { get; set; }
    public int Edad { get; set; }
    public string NombreVeterinario { get; set; }
    public string NumeroDocumentoVeterinario { get; set; }
    public string EspecialidadVeterinario { get; set; }
    public DateTime FechaConsulta { get; set; }
    public string MotivoConsulta { get; set; }
    public string Diagnostico { get; set; }
    public string Tratamiento { get; set; }
    public string Observaciones { get; set; }

}
