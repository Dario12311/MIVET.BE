using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Persintence;
using MIVET.BE.Repositorio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Enums;

namespace MIVET.BE.Repositorio
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly MIVETDbContext _context;

        public FacturaRepository(MIVETDbContext context)
        {
            _context = context;
        }

        #region CRUD Básico

        public async Task<Factura> ObtenerPorIdAsync(int id)
        {
            return await _context.Factura
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.Producto)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.ProcedimientoMedico)
                .Include(f => f.Cita)
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Factura>> ObtenerTodosAsync()
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<Factura> CrearAsync(Factura factura)
        {
            factura.FechaCreacion = DateTime.Now;
            factura.FechaFactura = DateTime.Now;
            factura.Estado = EstadoFactura.Pendiente;

            _context.Factura.Add(factura);
            await _context.SaveChangesAsync();
            return factura;
        }

        public async Task<Factura> ActualizarAsync(Factura factura)
        {
            factura.FechaModificacion = DateTime.Now;

            _context.Factura.Update(factura);
            await _context.SaveChangesAsync();
            return factura;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var factura = await _context.Factura.FindAsync(id);
            if (factura == null) return false;

            _context.Factura.Remove(factura);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Consultas específicas

        public async Task<Factura> ObtenerPorNumeroFacturaAsync(string numeroFactura)
        {
            return await _context.Factura
                .Include(f => f.DetallesFactura)
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .FirstOrDefaultAsync(f => f.NumeroFactura == numeroFactura);
        }

        public async Task<Factura> ObtenerPorCitaIdAsync(int citaId)
        {
            return await _context.Factura
                .Include(f => f.DetallesFactura)
                .FirstOrDefaultAsync(f => f.CitaId == citaId);
        }

        public async Task<IEnumerable<Factura>> ObtenerPorClienteAsync(string numeroDocumentoCliente)
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                .Include(f => f.MedicoVeterinario)
                .Where(f => f.NumeroDocumentoCliente == numeroDocumentoCliente)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> ObtenerPorVeterinarioAsync(string numeroDocumento)
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Where(f => f.MedicoVeterinarioNumeroDocumento == numeroDocumento)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> ObtenerPorFechaAsync(DateTime fecha)
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .Where(f => f.FechaFactura.Date == fecha.Date)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> ObtenerPorRangoFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .Where(f => f.FechaFactura.Date >= fechaInicio.Date && f.FechaFactura.Date <= fechaFin.Date)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> ObtenerPorEstadoAsync(EstadoFactura estado)
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .Where(f => f.Estado == estado)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        #endregion

        #region Consultas con Filtros

        public async Task<IEnumerable<Factura>> ObtenerPorFiltroAsync(FiltroFacturaDto filtro)
        {
            var query = _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtro.NumeroFactura))
                query = query.Where(f => f.NumeroFactura.Contains(filtro.NumeroFactura));

            if (filtro.CitaId.HasValue)
                query = query.Where(f => f.CitaId == filtro.CitaId.Value);

            if (!string.IsNullOrEmpty(filtro.NumeroDocumentoCliente))
                query = query.Where(f => f.NumeroDocumentoCliente == filtro.NumeroDocumentoCliente);

            if (!string.IsNullOrEmpty(filtro.MedicoVeterinarioNumeroDocumento))
                query = query.Where(f => f.MedicoVeterinarioNumeroDocumento == filtro.MedicoVeterinarioNumeroDocumento);

            if (filtro.FechaInicio.HasValue)
                query = query.Where(f => f.FechaFactura.Date >= filtro.FechaInicio.Value.Date);

            if (filtro.FechaFin.HasValue)
                query = query.Where(f => f.FechaFactura.Date <= filtro.FechaFin.Value.Date);

            if (filtro.Estado.HasValue)
                query = query.Where(f => f.Estado == filtro.Estado.Value);

            if (filtro.MetodoPago.HasValue)
                query = query.Where(f => f.MetodoPago == filtro.MetodoPago.Value);

            if (filtro.MontoMinimo.HasValue)
                query = query.Where(f => f.Total >= filtro.MontoMinimo.Value);

            if (filtro.MontoMaximo.HasValue)
                query = query.Where(f => f.Total <= filtro.MontoMaximo.Value);

            return await query
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> BuscarAsync(string termino)
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .Where(f => f.NumeroFactura.Contains(termino) ||
                           f.Mascota.Nombre.Contains(termino) ||
                           f.Mascota.PersonaCliente.PrimerNombre.Contains(termino) ||
                           f.Mascota.PersonaCliente.PrimerApellido.Contains(termino) ||
                           f.MedicoVeterinario.Nombre.Contains(termino))
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        #endregion

        #region Consultas Detalladas

        public async Task<FacturaDto> ObtenerDetalladaPorIdAsync(int id)
        {
            return await _context.Factura
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.Producto)
                .Include(f => f.DetallesFactura)
                    .ThenInclude(d => d.ProcedimientoMedico)
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .Where(f => f.Id == id)
                .Select(f => new FacturaDto
                {
                    Id = f.Id,
                    NumeroFactura = f.NumeroFactura,
                    CitaId = f.CitaId,
                    MascotaId = f.MascotaId,
                    NumeroDocumentoCliente = f.NumeroDocumentoCliente,
                    MedicoVeterinarioNumeroDocumento = f.MedicoVeterinarioNumeroDocumento,
                    FechaFactura = f.FechaFactura,
                    Subtotal = f.Subtotal,
                    DescuentoPorcentaje = f.DescuentoPorcentaje,
                    DescuentoValor = f.DescuentoValor,
                    IVA = f.IVA,
                    Total = f.Total,
                    Estado = f.Estado,
                    Observaciones = f.Observaciones,
                    MetodoPago = f.MetodoPago,
                    CreadoPor = f.CreadoPor,
                    FechaCreacion = f.FechaCreacion,
                    FechaModificacion = f.FechaModificacion,
                    ModificadoPor = f.ModificadoPor,
                    NombreMascota = f.Mascota.Nombre,
                    NombreCliente = f.Mascota.PersonaCliente.PrimerNombre + " " + f.Mascota.PersonaCliente.PrimerApellido,
                    NombreVeterinario = f.MedicoVeterinario.Nombre,
                    DetallesFactura = f.DetallesFactura.Select(d => new DetalleFacturaDto
                    {
                        Id = d.Id,
                        FacturaId = d.FacturaId,
                        TipoItem = d.TipoItem,
                        ProductoId = d.ProductoId,
                        ProcedimientoMedicoId = d.ProcedimientoMedicoId,
                        DescripcionItem = d.DescripcionItem,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        DescuentoPorcentaje = d.DescuentoPorcentaje,
                        Subtotal = d.Subtotal,
                        Total = d.Total,
                        Observaciones = d.Observaciones
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FacturaDto>> ObtenerConDetallesAsync()
        {
            return await _context.Factura
                .Include(f => f.Mascota)
                    .ThenInclude(m => m.PersonaCliente)
                .Include(f => f.MedicoVeterinario)
                .Select(f => new FacturaDto
                {
                    Id = f.Id,
                    NumeroFactura = f.NumeroFactura,
                    CitaId = f.CitaId,
                    MascotaId = f.MascotaId,
                    NumeroDocumentoCliente = f.NumeroDocumentoCliente,
                    MedicoVeterinarioNumeroDocumento = f.MedicoVeterinarioNumeroDocumento,
                    FechaFactura = f.FechaFactura,
                    Subtotal = f.Subtotal,
                    DescuentoPorcentaje = f.DescuentoPorcentaje,
                    DescuentoValor = f.DescuentoValor,
                    IVA = f.IVA,
                    Total = f.Total,
                    Estado = f.Estado,
                    Observaciones = f.Observaciones,
                    MetodoPago = f.MetodoPago,
                    CreadoPor = f.CreadoPor,
                    FechaCreacion = f.FechaCreacion,
                    FechaModificacion = f.FechaModificacion,
                    ModificadoPor = f.ModificadoPor,
                    NombreMascota = f.Mascota.Nombre,
                    NombreCliente = f.Mascota.PersonaCliente.PrimerNombre + " " + f.Mascota.PersonaCliente.PrimerApellido,
                    NombreVeterinario = f.MedicoVeterinario.Nombre
                })
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        #endregion

        #region Operaciones de Estado

        public async Task<bool> ActualizarEstadoAsync(int id, EstadoFactura estado, string modificadoPor)
        {
            var factura = await _context.Factura.FindAsync(id);
            if (factura == null) return false;

            factura.Estado = estado;
            factura.FechaModificacion = DateTime.Now;
            factura.ModificadoPor = modificadoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AnularFacturaAsync(int id, string anulacionMotivo, string anuladoPor)
        {
            var factura = await _context.Factura.FindAsync(id);
            if (factura == null) return false;

            factura.Estado = EstadoFactura.Anulada;
            factura.Observaciones = $"{factura.Observaciones} | ANULADA: {anulacionMotivo}";
            factura.FechaModificacion = DateTime.Now;
            factura.ModificadoPor = anuladoPor;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Generación de números

        public async Task<string> GenerarNumeroFacturaAsync()
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString("00");

            var ultimaFactura = await _context.Factura
                .Where(f => f.NumeroFactura.StartsWith($"F{year}{month}"))
                .OrderByDescending(f => f.NumeroFactura)
                .FirstOrDefaultAsync();

            int numeroConsecutivo = 1;
            if (ultimaFactura != null)
            {
                var ultimoNumero = ultimaFactura.NumeroFactura.Substring(7); // F202412
                if (int.TryParse(ultimoNumero, out int numero))
                {
                    numeroConsecutivo = numero + 1;
                }
            }

            return $"F{year}{month}{numeroConsecutivo:D4}";
        }

        #endregion

        #region Validaciones

        public async Task<bool> ExisteFacturaParaCitaAsync(int citaId)
        {
            return await _context.Factura.AnyAsync(f => f.CitaId == citaId);
        }

        public async Task<bool> ExisteNumeroFacturaAsync(string numeroFactura, int? excluirId = null)
        {
            var query = _context.Factura.AsQueryable();

            if (excluirId.HasValue)
                query = query.Where(f => f.Id != excluirId.Value);

            return await query.AnyAsync(f => f.NumeroFactura == numeroFactura);
        }

        #endregion

        #region Estadísticas

        public async Task<decimal> ObtenerTotalVentasPorFechaAsync(DateTime fecha)
        {
            return await _context.Factura
                .Where(f => f.FechaFactura.Date == fecha.Date && f.Estado == EstadoFactura.Pagada)
                .SumAsync(f => f.Total);
        }

        public async Task<decimal> ObtenerTotalVentasPorRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Factura
                .Where(f => f.FechaFactura.Date >= fechaInicio.Date &&
                           f.FechaFactura.Date <= fechaFin.Date &&
                           f.Estado == EstadoFactura.Pagada)
                .SumAsync(f => f.Total);
        }

        public async Task<int> ContarFacturasPorEstadoAsync(EstadoFactura estado)
        {
            return await _context.Factura.CountAsync(f => f.Estado == estado);
        }

        #endregion
    }
}
