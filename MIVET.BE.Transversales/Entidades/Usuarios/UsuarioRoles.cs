namespace MedicalWeb.BE.Transversales;

public class UsuarioRoles
{
    public int Id { get; set; } 
    public int UsuarioId { get; set; } 
    public int RolId { get; set; } 
    public string NombreUsuario { get; set; }
    public string Password { get; set; }
}