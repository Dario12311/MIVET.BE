using MIVET.BE.Transversales;

namespace MedicalWeb.BE.Transversales;

public class ActualizarRolesDTO
{
    public Usuarios UsuarioBase { get; set; }
    public List<int> RolesAAgregar { get; set; }
}