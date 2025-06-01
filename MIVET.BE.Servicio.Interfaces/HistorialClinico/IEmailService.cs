using MIVET.BE.Transversales.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio.Interfaces
{
    public interface IEmailService
    {
        // Configuración básica
        Task<bool> ConfigurarAsync(string servidor, int puerto, string usuario, string password, bool usarSSL = true);

        // Envío de emails
        Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true);
        Task<bool> EnviarEmailConArchivosAsync(string destinatario, string asunto, string cuerpo, List<string> rutasArchivos, bool esHtml = true);
        Task<bool> EnviarEmailMultiplesDestinatariosAsync(List<string> destinatarios, string asunto, string cuerpo, bool esHtml = true);

        // Plantillas específicas del sistema
        Task<bool> EnviarHistorialClinicoAsync(HistorialClinicoCompletoDto historial, List<string> destinatarios);
        Task<bool> EnviarFacturaAsync(FacturaDto factura, List<string> destinatarios);
        Task<bool> EnviarRecordatorioCitaAsync(CitaDetalladaDto cita, string destinatario);
        Task<bool> EnviarNotificacionCitaCompletadaAsync(HistorialClinicoCompletoDto historial, FacturaDto factura, List<string> destinatarios);

        // Utilidades
        Task<string> GenerarPlantillaHistorialClinicoAsync(HistorialClinicoCompletoDto historial);
        Task<string> GenerarPlantillaFacturaAsync(FacturaDto factura);
        Task<bool> ValidarEmailAsync(string email);
    }
}
