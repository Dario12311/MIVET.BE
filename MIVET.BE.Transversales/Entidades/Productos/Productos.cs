using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales;

public class Productos
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string Categoria { get; set; }
    public string Estado { get; set; } // Activo, Inactivo, etc.

    // Guarda la ruta o URL de la imagen
    public string ImagenUrl { get; set; }
}

