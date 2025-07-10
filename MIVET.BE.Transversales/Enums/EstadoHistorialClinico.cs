using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Enums
{
    public enum EstadoHistorialClinico
    {
        Borrador = 1,
        Completado = 2,
        Modificado = 3,
        Cancelado = 4
    }

    public enum EstadoFactura
    {
        Pendiente = 1,
        Pagada = 2,
        Parcial = 3,
        Cancelada = 4,
        Anulada = 5
    }

    public enum MetodoPago
    {
        Efectivo = 1,
        TarjetaCredito = 2,
        TarjetaDebito = 3,
        Transferencia = 4,
        Cheque = 5,
        Mixto = 6
    }

    public enum TipoItemFactura
    {
        Consulta = 1,
        Producto = 2,
        Procedimiento = 3,
        Cirugia = 4,
        Medicamento = 5,
        Examen = 6,
        Otro = 7
    }
}