using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Repositorio.Interfaces
{
    public interface IHistorialClinicoRepository
    {
        // CRUD Básico
        Task<HistorialClinico> ObtenerPorIdAsync(int id);
        Task<IEnumerable<HistorialClinico>> ObtenerTodosAsync();
        Task<HistorialClinico> CrearAsync(HistorialClinico historialClinico);
        Task<HistorialClinico> ActualizarAsync(HistorialClinico historialClinico);
        Task<bool> EliminarAsync(int id);

        // Consultas por Cita
        Task<HistorialClinico> ObtenerPorCitaIdAsync(int citaId);
        Task<bool> ExisteHistorialParaCitaAsync(int citaId);

        // Consultas por Mascota
        Task<IEnumerable<HistorialClinico>> ObtenerPorMascotaIdAsync(int mascotaId);
        Task<IEnumerable<HistorialClinico>> ObtenerHistorialCompletoMascotaAsync(int mascotaId);

        // Consultas por Veterinario
        Task<IEnumerable<HistorialClinico>> ObtenerPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<HistorialClinico>> ObtenerPorVeterinarioYFechaAsync(string numeroDocumento, DateTime fecha);

        // Consultas por Cliente
        Task<IEnumerable<HistorialClinico>> ObtenerPorClienteAsync(string numeroDocumentoCliente);

        // Consultas con Filtros
        Task<IEnumerable<HistorialClinico>> ObtenerPorFiltroAsync(FiltroHistorialClinicoDto filtro);
        Task<IEnumerable<HistorialClinico>> BuscarAsync(string termino);

        // Consultas Detalladas
        Task<HistorialClinicoDto> ObtenerDetalladoPorIdAsync(int id);
        Task<IEnumerable<HistorialClinicoDto>> ObtenerConDetallesAsync();
        Task<HistorialClinicoCompletoDto> ObtenerCompletoConHistorialAsync(int id);

        // Validaciones
        Task<bool> PuedeCrearHistorialAsync(int citaId, string veterinarioNumeroDocumento);
        Task<bool> PuedeModificarHistorialAsync(int id, string veterinarioNumeroDocumento);

        // Operaciones de Estado
        Task<bool> CompletarHistorialAsync(int id, string completadoPor);
        Task<bool> CancelarHistorialAsync(int id, string canceladoPor);

        // Estadísticas
        Task<int> ContarHistorialesPorMascotaAsync(int mascotaId);
        Task<int> ContarHistorialesPorVeterinarioAsync(string numeroDocumento);
    }
}