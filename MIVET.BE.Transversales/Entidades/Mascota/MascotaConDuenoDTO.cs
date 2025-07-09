using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales;

public class MascotaConDuenoDTO
{
    public int id { get; set; }
    public string Nombre { get; set; }
    public string Especie { get; set; }
    public string Raza { get; set; }
    public int Edad { get; set; }
    public string Genero { get; set; }
    public char Estado { get; set; }
    public string NumeroDocumento{ get; set; }
    public string PrimerNombreDueno { get; set; }
    public string PrimerApellidoDueno { get; set; }
    public string SegundoApellidoDueno { get; set; }
}
