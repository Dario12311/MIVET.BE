using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Repositorio.Interfaces
{
    public interface IProcedimientoMedicoRepository
    {
        // CRUD Básico
        Task<ProcedimientoMedico> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ProcedimientoMedico>> ObtenerTodosAsync();
        Task<ProcedimientoMedico> CrearAsync(ProcedimientoMedico procedimiento);
        Task<ProcedimientoMedico> ActualizarAsync(ProcedimientoMedico procedimiento);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas
        Task<IEnumerable<ProcedimientoMedico>> ObtenerActivosAsync();
        Task<IEnumerable<ProcedimientoMedico>> ObtenerPorCategoriaAsync(string categoria);
        Task<ProcedimientoMedico> ObtenerPorNombreAsync(string nombre);

        // Operaciones
        Task<bool> ActivarDesactivarAsync(int id, bool esActivo, string modificadoPor);
        Task<bool> ActualizarPrecioAsync(int id, decimal nuevoPrecio, string modificadoPor);

        // Validaciones
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    }
}
