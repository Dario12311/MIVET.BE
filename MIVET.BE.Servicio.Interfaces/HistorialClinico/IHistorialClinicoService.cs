using MIVET.BE.Transversales.DTOs;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface IHistorialClinicoService
    {
        // CRUD Básico
        Task<HistorialClinicoDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<HistorialClinicoDto>> ObtenerTodosAsync();
        Task<HistorialClinicoDto> CrearAsync(CrearHistorialClinicoDto dto);
        Task<HistorialClinicoDto> ActualizarAsync(ActualizarHistorialClinicoDto dto);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas
        Task<HistorialClinicoDto> ObtenerPorCitaIdAsync(int citaId);
        Task<IEnumerable<HistorialClinicoDto>> ObtenerPorMascotaIdAsync(int mascotaId);
        Task<IEnumerable<HistorialClinicoDto>> ObtenerPorVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<HistorialClinicoDto>> ObtenerPorClienteAsync(string numeroDocumentoCliente);

        // Consultas avanzadas
        Task<HistorialClinicoCompletoDto> ObtenerCompletoConHistorialAsync(int id);
        Task<IEnumerable<HistorialClinicoDto>> ObtenerPorFiltroAsync(FiltroHistorialClinicoDto filtro);
        Task<IEnumerable<HistorialClinicoDto>> BuscarAsync(string termino);

        // Operaciones especiales
        Task<bool> CompletarHistorialAsync(int id, string completadoPor);
        Task<bool> CancelarHistorialAsync(int id, string canceladoPor);
        Task<bool> IniciarCitaAsync(int citaId, string veterinarioNumeroDocumento);

        // Validaciones
        Task<bool> PuedeCrearHistorialAsync(int citaId, string veterinarioNumeroDocumento);
        Task<bool> PuedeModificarHistorialAsync(int id, string veterinarioNumeroDocumento);

        // Reportes
        Task<IEnumerable<HistorialClinicoDto>> ObtenerHistorialCompletoMascotaAsync(int mascotaId);
        Task<int> ContarHistorialesPorMascotaAsync(int mascotaId);
    }
}
