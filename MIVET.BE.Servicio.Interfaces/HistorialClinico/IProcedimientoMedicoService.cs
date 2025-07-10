using MIVET.BE.Transversales.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface IProcedimientoMedicoService
    {
        // CRUD Básico
        Task<ProcedimientoMedicoDto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<ProcedimientoMedicoDto>> ObtenerTodosAsync();
        Task<ProcedimientoMedicoDto> CrearAsync(CrearProcedimientoMedicoDto dto);
        Task<ProcedimientoMedicoDto> ActualizarAsync(ActualizarProcedimientoMedicoDto dto);
        Task<bool> EliminarAsync(int id);

        // Consultas específicas
        Task<IEnumerable<ProcedimientoMedicoDto>> ObtenerActivosAsync();
        Task<IEnumerable<ProcedimientoMedicoDto>> ObtenerPorCategoriaAsync(string categoria);
        Task<ProcedimientoMedicoDto> ObtenerPorNombreAsync(string nombre);

        // Operaciones
        Task<bool> ActivarDesactivarAsync(int id, bool esActivo, string modificadoPor);
        Task<bool> ActualizarPrecioAsync(int id, decimal nuevoPrecio, string modificadoPor);

        // Validaciones
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    }
}
