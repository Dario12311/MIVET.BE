using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Servicio
{
    public class ProcedimientoMedicoService : IProcedimientoMedicoService
    {
        private readonly IProcedimientoMedicoRepository _repository;

        public ProcedimientoMedicoService(IProcedimientoMedicoRepository repository)
        {
            _repository = repository;
        }

        #region CRUD Básico

        public async Task<ProcedimientoMedicoDto> ObtenerPorIdAsync(int id)
        {
            var procedimiento = await _repository.ObtenerPorIdAsync(id);
            if (procedimiento == null)
                throw new KeyNotFoundException($"No se encontró el procedimiento médico con ID: {id}");

            return MapearADto(procedimiento);
        }

        public async Task<IEnumerable<ProcedimientoMedicoDto>> ObtenerTodosAsync()
        {
            var procedimientos = await _repository.ObtenerTodosAsync();
            return procedimientos.Select(MapearADto);
        }

        public async Task<ProcedimientoMedicoDto> CrearAsync(CrearProcedimientoMedicoDto dto)
        {
            try
            {
                // Validar que no existe el nombre
                if (await ExisteNombreAsync(dto.Nombre))
                    throw new InvalidOperationException($"Ya existe un procedimiento médico con el nombre: {dto.Nombre}");

                var procedimiento = new ProcedimientoMedico
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Precio = dto.Precio,
                    Categoria = dto.Categoria,
                    CreadoPor = dto.CreadoPor
                };

                var procedimientoCreado = await _repository.CrearAsync(procedimiento);
                return MapearADto(procedimientoCreado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el procedimiento médico", ex);
            }
        }

        public async Task<ProcedimientoMedicoDto> ActualizarAsync(ActualizarProcedimientoMedicoDto dto)
        {
            try
            {
                var procedimiento = await _repository.ObtenerPorIdAsync(dto.Id);
                if (procedimiento == null)
                    throw new KeyNotFoundException($"No se encontró el procedimiento médico con ID: {dto.Id}");

                // Validar que no existe el nombre (excluyendo el actual)
                if (await ExisteNombreAsync(dto.Nombre, dto.Id))
                    throw new InvalidOperationException($"Ya existe un procedimiento médico con el nombre: {dto.Nombre}");

                procedimiento.Nombre = dto.Nombre;
                procedimiento.Descripcion = dto.Descripcion;
                procedimiento.Precio = dto.Precio;
                procedimiento.Categoria = dto.Categoria;
                procedimiento.EsActivo = dto.EsActivo;
                procedimiento.ModificadoPor = dto.ModificadoPor;

                var procedimientoActualizado = await _repository.ActualizarAsync(procedimiento);
                return MapearADto(procedimientoActualizado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el procedimiento médico", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _repository.EliminarAsync(id);
        }

        #endregion

        #region Consultas específicas

        public async Task<IEnumerable<ProcedimientoMedicoDto>> ObtenerActivosAsync()
        {
            var procedimientos = await _repository.ObtenerActivosAsync();
            return procedimientos.Select(MapearADto);
        }

        public async Task<IEnumerable<ProcedimientoMedicoDto>> ObtenerPorCategoriaAsync(string categoria)
        {
            var procedimientos = await _repository.ObtenerPorCategoriaAsync(categoria);
            return procedimientos.Select(MapearADto);
        }

        public async Task<ProcedimientoMedicoDto> ObtenerPorNombreAsync(string nombre)
        {
            var procedimiento = await _repository.ObtenerPorNombreAsync(nombre);
            return procedimiento != null ? MapearADto(procedimiento) : null;
        }

        #endregion

        #region Operaciones

        public async Task<bool> ActivarDesactivarAsync(int id, bool esActivo, string modificadoPor)
        {
            return await _repository.ActivarDesactivarAsync(id, esActivo, modificadoPor);
        }

        public async Task<bool> ActualizarPrecioAsync(int id, decimal nuevoPrecio, string modificadoPor)
        {
            return await _repository.ActualizarPrecioAsync(id, nuevoPrecio, modificadoPor);
        }

        #endregion

        #region Validaciones

        public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null)
        {
            return await _repository.ExisteNombreAsync(nombre, excluirId);
        }

        #endregion

        #region Mapeo

        private static ProcedimientoMedicoDto MapearADto(ProcedimientoMedico procedimiento)
        {
            return new ProcedimientoMedicoDto
            {
                Id = procedimiento.Id,
                Nombre = procedimiento.Nombre,
                Descripcion = procedimiento.Descripcion,
                Precio = procedimiento.Precio,
                Categoria = procedimiento.Categoria,
                EsActivo = procedimiento.EsActivo,
                FechaCreacion = procedimiento.FechaCreacion,
                FechaModificacion = procedimiento.FechaModificacion,
                CreadoPor = procedimiento.CreadoPor,
                ModificadoPor = procedimiento.ModificadoPor
            };
        }

        #endregion
    }
}
