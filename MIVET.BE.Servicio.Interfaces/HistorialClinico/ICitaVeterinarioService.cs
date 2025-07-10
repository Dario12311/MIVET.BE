using MIVET.BE.Transversales.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface ICitaVeterinarioService
    {
        // Gestión de citas del veterinario
        Task<IEnumerable<CitaDto>> ObtenerCitasVeterinarioAsync(string numeroDocumento);
        Task<IEnumerable<CitaDto>> ObtenerCitasDelDiaAsync(string numeroDocumento, DateTime fecha);
        Task<IEnumerable<CitaDto>> ObtenerProximasCitasAsync(string numeroDocumento, int diasAdelante = 7);
        Task<IEnumerable<CitaDto>> ObtenerCitasCompletadasAsync(string numeroDocumento, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        // Gestión del proceso de cita
        Task<CitaDetalladaDto> IniciarCitaAsync(int citaId, string veterinarioNumeroDocumento);
        Task<bool> CompletarCitaAsync(int citaId, string veterinarioNumeroDocumento);
        Task<bool> CancelarCitaAsync(int citaId, string motivoCancelacion, string veterinarioNumeroDocumento);

        // Información completa de la cita
        Task<CitaDetalladaDto> ObtenerCitaCompletaAsync(int citaId);
        Task<HistorialClinicoCompletoDto> ObtenerHistorialCompletoCitaAsync(int citaId);

        // Validaciones
        Task<bool> PuedeIniciarCitaAsync(int citaId, string veterinarioNumeroDocumento);
        Task<bool> PuedeCompletarCitaAsync(int citaId, string veterinarioNumeroDocumento);
    }
}
