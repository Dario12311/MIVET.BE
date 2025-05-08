using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class MascotaDTO
{
    public int Id { get; set; }
    public String Nombre { get; set; }
    public String Especie { get; set; }
    public String Raza { get; set; }
    public int Edad { get; set; }
    public String Genero { get; set; }
    public string NumeroDocumento { get; set; }
    public char Estado { get; set; }
}
