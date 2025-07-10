using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.Entidades;

namespace MIVET.BE.Repositorio
{
    public class ProcedimientoMedicoRepository : IProcedimientoMedicoRepository
    {
        private readonly MIVETDbContext _context;

        public ProcedimientoMedicoRepository(MIVETDbContext context)
        {
            _context = context;
        }

        #region CRUD Básico

        public async Task<ProcedimientoMedico> ObtenerPorIdAsync(int id)
        {
            return await _context.ProcedimientoMedico.FindAsync(id);
        }

        public async Task<IEnumerable<ProcedimientoMedico>> ObtenerTodosAsync()
        {
            try
            {
                return await _context.ProcedimientoMedico
                    .OrderBy(p => p.Categoria)
                    .ThenBy(p => p.Nombre)
                    .ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<ProcedimientoMedico> CrearAsync(ProcedimientoMedico procedimiento)
        {
            procedimiento.FechaCreacion = DateTime.Now;
            procedimiento.EsActivo = true;

            _context.ProcedimientoMedico.Add(procedimiento);
            await _context.SaveChangesAsync();
            return procedimiento;
        }

        public async Task<ProcedimientoMedico> ActualizarAsync(ProcedimientoMedico procedimiento)
        {
            procedimiento.FechaModificacion = DateTime.Now;

            _context.ProcedimientoMedico.Update(procedimiento);
            await _context.SaveChangesAsync();
            return procedimiento;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var procedimiento = await _context.ProcedimientoMedico.FindAsync(id);
            if (procedimiento == null) return false;

            _context.ProcedimientoMedico.Remove(procedimiento);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas específicas

        public async Task<IEnumerable<ProcedimientoMedico>> ObtenerActivosAsync()
        {
            return await _context.ProcedimientoMedico
                .Where(p => p.EsActivo)
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcedimientoMedico>> ObtenerPorCategoriaAsync(string categoria)
        {
            return await _context.ProcedimientoMedico
                .Where(p => p.Categoria == categoria && p.EsActivo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<ProcedimientoMedico> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.ProcedimientoMedico
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == nombre.ToLower());
        }

        #endregion

        #region Operaciones

        public async Task<bool> ActivarDesactivarAsync(int id, bool esActivo, string modificadoPor)
        {
            var procedimiento = await _context.ProcedimientoMedico.FindAsync(id);
            if (procedimiento == null) return false;

            procedimiento.EsActivo = esActivo;
            procedimiento.FechaModificacion = DateTime.Now;
            procedimiento.ModificadoPor = modificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarPrecioAsync(int id, decimal nuevoPrecio, string modificadoPor)
        {
            var procedimiento = await _context.ProcedimientoMedico.FindAsync(id);
            if (procedimiento == null) return false;

            procedimiento.Precio = nuevoPrecio;
            procedimiento.FechaModificacion = DateTime.Now;
            procedimiento.ModificadoPor = modificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Validaciones

        public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null)
        {
            var query = _context.ProcedimientoMedico.AsQueryable();

            if (excluirId.HasValue)
                query = query.Where(p => p.Id != excluirId.Value);

            return await query.AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower());
        }

        #endregion
    }
}
