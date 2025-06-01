using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MIVET.BE.Servicio.Interfaces;
using MIVET.BE.Transversales.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MIVET.BE.Servicio
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private string _smtpServer;
        private int _smtpPort;
        private string _smtpUsername;
        private string _smtpPassword;
        private bool _enableSsl;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            CargarConfiguracion();
        }

        #region Configuración

        private void CargarConfiguracion()
        {
            // NOTA: Configurar estas credenciales en appsettings.json o variables de entorno
            // Ejemplo de configuración en appsettings.json:
            /*
            "EmailSettings": {
                "SmtpServer": "smtp.gmail.com",
                "SmtpPort": 587,
                "SmtpUsername": "tu-email@gmail.com",
                "SmtpPassword": "tu-contraseña-de-aplicacion",
                "EnableSsl": true
            }
            */

            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
        }

        public async Task<bool> ConfigurarAsync(string servidor, int puerto, string usuario, string password, bool usarSSL = true)
        {
            try
            {
                _smtpServer = servidor;
                _smtpPort = puerto;
                _smtpUsername = usuario;
                _smtpPassword = password;
                _enableSsl = usarSSL;

                _logger.LogInformation("Configuración SMTP actualizada correctamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al configurar SMTP");
                return false;
            }
        }

        #endregion

        #region Envío de emails básico

        public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string cuerpo, bool esHtml = true)
        {
            try
            {
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    _logger.LogWarning("Credenciales SMTP no configuradas");
                    return false;
                }

                using var cliente = new SmtpClient(_smtpServer, _smtpPort);
                cliente.EnableSsl = _enableSsl;
                cliente.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                using var mensaje = new MailMessage();
                mensaje.From = new MailAddress(_smtpUsername, "Sistema MIVET");
                mensaje.To.Add(destinatario);
                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.IsBodyHtml = esHtml;
                mensaje.BodyEncoding = Encoding.UTF8;

                await cliente.SendMailAsync(mensaje);
                _logger.LogInformation($"Email enviado exitosamente a: {destinatario}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar email a: {destinatario}");
                return false;
            }
        }

        public async Task<bool> EnviarEmailConArchivosAsync(string destinatario, string asunto, string cuerpo, List<string> rutasArchivos, bool esHtml = true)
        {
            try
            {
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    _logger.LogWarning("Credenciales SMTP no configuradas");
                    return false;
                }

                using var cliente = new SmtpClient(_smtpServer, _smtpPort);
                cliente.EnableSsl = _enableSsl;
                cliente.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                using var mensaje = new MailMessage();
                mensaje.From = new MailAddress(_smtpUsername, "Sistema MIVET");
                mensaje.To.Add(destinatario);
                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.IsBodyHtml = esHtml;
                mensaje.BodyEncoding = Encoding.UTF8;

                // Agregar archivos adjuntos
                foreach (var rutaArchivo in rutasArchivos)
                {
                    if (File.Exists(rutaArchivo))
                    {
                        var attachment = new Attachment(rutaArchivo);
                        mensaje.Attachments.Add(attachment);
                    }
                }

                await cliente.SendMailAsync(mensaje);
                _logger.LogInformation($"Email con archivos enviado exitosamente a: {destinatario}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar email con archivos a: {destinatario}");
                return false;
            }
        }

        public async Task<bool> EnviarEmailMultiplesDestinatariosAsync(List<string> destinatarios, string asunto, string cuerpo, bool esHtml = true)
        {
            try
            {
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    _logger.LogWarning("Credenciales SMTP no configuradas");
                    return false;
                }

                using var cliente = new SmtpClient(_smtpServer, _smtpPort);
                cliente.EnableSsl = _enableSsl;
                cliente.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                using var mensaje = new MailMessage();
                mensaje.From = new MailAddress(_smtpUsername, "Sistema MIVET");

                foreach (var destinatario in destinatarios)
                {
                    if (await ValidarEmailAsync(destinatario))
                    {
                        mensaje.To.Add(destinatario);
                    }
                }

                if (mensaje.To.Count == 0)
                {
                    _logger.LogWarning("No hay destinatarios válidos");
                    return false;
                }

                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.IsBodyHtml = esHtml;
                mensaje.BodyEncoding = Encoding.UTF8;

                await cliente.SendMailAsync(mensaje);
                _logger.LogInformation($"Email enviado exitosamente a {mensaje.To.Count} destinatarios");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a múltiples destinatarios");
                return false;
            }
        }

        #endregion

        #region Plantillas específicas del sistema

        public async Task<bool> EnviarHistorialClinicoAsync(HistorialClinicoCompletoDto historial, List<string> destinatarios)
        {
            try
            {
                var asunto = $"Historia Clínica - {historial.HistorialClinico.NombreMascota} - {historial.HistorialClinico.FechaRegistro:dd/MM/yyyy}";
                var cuerpo = await GenerarPlantillaHistorialClinicoAsync(historial);

                return await EnviarEmailMultiplesDestinatariosAsync(destinatarios, asunto, cuerpo, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar historial clínico por email");
                return false;
            }
        }

        public async Task<bool> EnviarFacturaAsync(FacturaDto factura, List<string> destinatarios)
        {
            try
            {
                var asunto = $"Factura {factura.NumeroFactura} - {factura.NombreMascota}";
                var cuerpo = await GenerarPlantillaFacturaAsync(factura);

                return await EnviarEmailMultiplesDestinatariosAsync(destinatarios, asunto, cuerpo, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar factura por email");
                return false;
            }
        }

        public async Task<bool> EnviarRecordatorioCitaAsync(CitaDetalladaDto cita, string destinatario)
        {
            try
            {
                var asunto = $"Recordatorio de Cita - {cita.NombreMascota} - {cita.FechaCita:dd/MM/yyyy}";
                var cuerpo = GenerarPlantillaRecordatorioCita(cita);

                return await EnviarEmailAsync(destinatario, asunto, cuerpo, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar recordatorio de cita");
                return false;
            }
        }

        public async Task<bool> EnviarNotificacionCitaCompletadaAsync(HistorialClinicoCompletoDto historial, FacturaDto factura, List<string> destinatarios)
        {
            try
            {
                var asunto = $"Cita Completada - {historial.HistorialClinico.NombreMascota} - {historial.HistorialClinico.FechaRegistro:dd/MM/yyyy}";
                var cuerpo = await GenerarPlantillaCitaCompletadaAsync(historial, factura);

                return await EnviarEmailMultiplesDestinatariosAsync(destinatarios, asunto, cuerpo, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de cita completada");
                return false;
            }
        }

        #endregion

        #region Generación de plantillas

        public async Task<string> GenerarPlantillaHistorialClinicoAsync(HistorialClinicoCompletoDto historial)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='es'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("    <title>Historia Clínica</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }");
            html.AppendLine("        .container { max-width: 800px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }");
            html.AppendLine("        .header { text-align: center; border-bottom: 2px solid #2c5aa0; padding-bottom: 20px; margin-bottom: 30px; }");
            html.AppendLine("        .logo { font-size: 28px; font-weight: bold; color: #2c5aa0; margin-bottom: 10px; }");
            html.AppendLine("        .section { margin-bottom: 25px; }");
            html.AppendLine("        .section-title { font-size: 18px; font-weight: bold; color: #2c5aa0; border-bottom: 1px solid #ddd; padding-bottom: 5px; margin-bottom: 15px; }");
            html.AppendLine("        .info-row { display: flex; margin-bottom: 8px; }");
            html.AppendLine("        .info-label { font-weight: bold; width: 200px; color: #555; }");
            html.AppendLine("        .info-value { flex: 1; }");
            html.AppendLine("        .text-area { background-color: #f8f9fa; padding: 15px; border-radius: 5px; border-left: 4px solid #2c5aa0; margin: 10px 0; }");
            html.AppendLine("        .footer { text-align: center; margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666; }");
            html.AppendLine("        .historial-anterior { background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 10px 0; }");
            html.AppendLine("        .historial-item { border-bottom: 1px solid #ddd; padding: 10px 0; }");
            html.AppendLine("        .historial-item:last-child { border-bottom: none; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class='container'>");

            // Header
            html.AppendLine("        <div class='header'>");
            html.AppendLine("            <div class='logo'>MIVET - Sistema Veterinario</div>");
            html.AppendLine("            <h2>Historia Clínica</h2>");
            html.AppendLine($"            <p>Fecha: {historial.HistorialClinico.FechaRegistro:dd/MM/yyyy HH:mm}</p>");
            html.AppendLine("        </div>");

            // Información del paciente
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <div class='section-title'>Información del Paciente</div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Mascota:</div><div class='info-value'>{historial.HistorialClinico.NombreMascota}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Especie:</div><div class='info-value'>{historial.HistorialClinico.EspecieMascota}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Raza:</div><div class='info-value'>{historial.HistorialClinico.RazaMascota}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Propietario:</div><div class='info-value'>{historial.HistorialClinico.NombreCliente}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Documento:</div><div class='info-value'>{historial.HistorialClinico.NumeroDocumentoCliente}</div></div>");
            html.AppendLine("        </div>");

            // Información del veterinario
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <div class='section-title'>Información del Veterinario</div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Veterinario:</div><div class='info-value'>{historial.HistorialClinico.NombreVeterinario}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Fecha de Consulta:</div><div class='info-value'>{historial.HistorialClinico.FechaCita:dd/MM/yyyy} - {historial.HistorialClinico.HoraCita}</div></div>");
            html.AppendLine("        </div>");

            // Información clínica
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <div class='section-title'>Información Clínica</div>");

            if (!string.IsNullOrEmpty(historial.HistorialClinico.MotivoConsulta))
            {
                html.AppendLine("            <h4>Motivo de Consulta:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.MotivoConsulta}</div>");
            }

            if (!string.IsNullOrEmpty(historial.HistorialClinico.Sintomas))
            {
                html.AppendLine("            <h4>Síntomas:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.Sintomas}</div>");
            }

            html.AppendLine($"            <div class='info-row'><div class='info-label'>Temperatura:</div><div class='info-value'>{historial.HistorialClinico.Temperatura ?? "No registrada"}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Peso:</div><div class='info-value'>{historial.HistorialClinico.Peso ?? "No registrado"}</div></div>");
            html.AppendLine($"            <div class='info-row'><div class='info-label'>Signos Vitales:</div><div class='info-value'>{historial.HistorialClinico.SignosVitales ?? "No registrados"}</div></div>");

            if (!string.IsNullOrEmpty(historial.HistorialClinico.ExamenFisico))
            {
                html.AppendLine("            <h4>Examen Físico:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.ExamenFisico}</div>");
            }

            if (!string.IsNullOrEmpty(historial.HistorialClinico.Diagnostico))
            {
                html.AppendLine("            <h4>Diagnóstico:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.Diagnostico}</div>");
            }

            if (!string.IsNullOrEmpty(historial.HistorialClinico.Tratamiento))
            {
                html.AppendLine("            <h4>Tratamiento:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.Tratamiento}</div>");
            }

            if (!string.IsNullOrEmpty(historial.HistorialClinico.Medicamentos))
            {
                html.AppendLine("            <h4>Medicamentos:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.Medicamentos}</div>");
            }

            if (!string.IsNullOrEmpty(historial.HistorialClinico.RecomendacionesGenerales))
            {
                html.AppendLine("            <h4>Recomendaciones:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.RecomendacionesGenerales}</div>");
            }

            if (!string.IsNullOrEmpty(historial.HistorialClinico.Observaciones))
            {
                html.AppendLine("            <h4>Observaciones:</h4>");
                html.AppendLine($"            <div class='text-area'>{historial.HistorialClinico.Observaciones}</div>");
            }

            if (historial.HistorialClinico.ProximaCita.HasValue)
            {
                html.AppendLine($"            <div class='info-row'><div class='info-label'>Próxima Cita:</div><div class='info-value'>{historial.HistorialClinico.ProximaCita.Value:dd/MM/yyyy}</div></div>");
            }
            html.AppendLine("        </div>");

            // Historial anterior (si existe)
            if (historial.HistorialAnterior != null && historial.HistorialAnterior.Any())
            {
                html.AppendLine("        <div class='section'>");
                html.AppendLine("            <div class='section-title'>Historial Anterior</div>");
                html.AppendLine("            <div class='historial-anterior'>");

                foreach (var item in historial.HistorialAnterior.Take(5)) // Mostrar solo los últimos 5
                {
                    html.AppendLine("                <div class='historial-item'>");
                    html.AppendLine($"                    <strong>Fecha:</strong> {item.FechaRegistro:dd/MM/yyyy} | <strong>Veterinario:</strong> {item.NombreVeterinario}");
                    if (!string.IsNullOrEmpty(item.Diagnostico))
                    {
                        html.AppendLine($"                    <br><strong>Diagnóstico:</strong> {item.Diagnostico}");
                    }
                    if (!string.IsNullOrEmpty(item.Tratamiento))
                    {
                        html.AppendLine($"                    <br><strong>Tratamiento:</strong> {item.Tratamiento}");
                    }
                    html.AppendLine("                </div>");
                }

                html.AppendLine("            </div>");
                html.AppendLine("        </div>");
            }

            // Footer
            html.AppendLine("        <div class='footer'>");
            html.AppendLine("            <p>Este documento ha sido generado automáticamente por el Sistema MIVET</p>");
            html.AppendLine($"            <p>Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            html.AppendLine("        </div>");

            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        public async Task<string> GenerarPlantillaFacturaAsync(FacturaDto factura)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='es'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("    <title>Factura</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }");
            html.AppendLine("        .container { max-width: 800px; margin: 0 auto; background-color: white; padding: 30px; border: 1px solid #ddd; }");
            html.AppendLine("        .header { text-align: center; border-bottom: 2px solid #2c5aa0; padding-bottom: 20px; margin-bottom: 30px; }");
            html.AppendLine("        .info-section { display: flex; justify-content: space-between; margin-bottom: 30px; }");
            html.AppendLine("        .info-column { width: 48%; }");
            html.AppendLine("        .table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }");
            html.AppendLine("        .table th, .table td { border: 1px solid #ddd; padding: 10px; text-align: left; }");
            html.AppendLine("        .table th { background-color: #2c5aa0; color: white; }");
            html.AppendLine("        .totals { text-align: right; }");
            html.AppendLine("        .total-row { font-weight: bold; font-size: 16px; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class='container'>");

            // Header
            html.AppendLine("        <div class='header'>");
            html.AppendLine("            <h1>MIVET - Sistema Veterinario</h1>");
            html.AppendLine($"            <h2>FACTURA {factura.NumeroFactura}</h2>");
            html.AppendLine($"            <p>Fecha: {factura.FechaFactura:dd/MM/yyyy}</p>");
            html.AppendLine("        </div>");

            // Información cliente y veterinario
            html.AppendLine("        <div class='info-section'>");
            html.AppendLine("            <div class='info-column'>");
            html.AppendLine("                <h3>Cliente:</h3>");
            html.AppendLine($"                <p><strong>{factura.NombreCliente}</strong></p>");
            html.AppendLine($"                <p>Documento: {factura.NumeroDocumentoCliente}</p>");
            html.AppendLine($"                <p>Mascota: {factura.NombreMascota}</p>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class='info-column'>");
            html.AppendLine("                <h3>Veterinario:</h3>");
            html.AppendLine($"                <p><strong>{factura.NombreVeterinario}</strong></p>");
            html.AppendLine($"                <p>Estado: {factura.Estado}</p>");
            html.AppendLine($"                <p>Método de Pago: {factura.MetodoPago}</p>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            // Detalles de la factura
            html.AppendLine("        <table class='table'>");
            html.AppendLine("            <thead>");
            html.AppendLine("                <tr>");
            html.AppendLine("                    <th>Descripción</th>");
            html.AppendLine("                    <th>Tipo</th>");
            html.AppendLine("                    <th>Cantidad</th>");
            html.AppendLine("                    <th>Precio Unit.</th>");
            html.AppendLine("                    <th>Descuento</th>");
            html.AppendLine("                    <th>Total</th>");
            html.AppendLine("                </tr>");
            html.AppendLine("            </thead>");
            html.AppendLine("            <tbody>");

            foreach (var detalle in factura.DetallesFactura)
            {
                html.AppendLine("                <tr>");
                html.AppendLine($"                    <td>{detalle.DescripcionItem}</td>");
                html.AppendLine($"                    <td>{detalle.TipoItem}</td>");
                html.AppendLine($"                    <td>{detalle.Cantidad}</td>");
                html.AppendLine($"                    <td>${detalle.PrecioUnitario:N2}</td>");
                html.AppendLine($"                    <td>{detalle.DescuentoPorcentaje}%</td>");
                html.AppendLine($"                    <td>${detalle.Total:N2}</td>");
                html.AppendLine("                </tr>");
            }

            html.AppendLine("            </tbody>");
            html.AppendLine("        </table>");

            // Totales
            html.AppendLine("        <div class='totals'>");
            html.AppendLine($"            <p>Subtotal: ${factura.Subtotal:N2}</p>");
            if (factura.DescuentoValor > 0)
            {
                html.AppendLine($"            <p>Descuento: -${factura.DescuentoValor:N2}</p>");
            }
            html.AppendLine($"            <p>IVA (19%): ${factura.IVA:N2}</p>");
            html.AppendLine($"            <p class='total-row'>TOTAL: ${factura.Total:N2}</p>");
            html.AppendLine("        </div>");

            if (!string.IsNullOrEmpty(factura.Observaciones))
            {
                html.AppendLine("        <div style='margin-top: 30px;'>");
                html.AppendLine("            <h4>Observaciones:</h4>");
                html.AppendLine($"            <p>{factura.Observaciones}</p>");
                html.AppendLine("        </div>");
            }

            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string GenerarPlantillaRecordatorioCita(CitaDetalladaDto cita)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='es'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <title>Recordatorio de Cita</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }");
            html.AppendLine("        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            html.AppendLine("        .header { text-align: center; color: #2c5aa0; margin-bottom: 30px; }");
            html.AppendLine("        .cita-info { background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class='container'>");
            html.AppendLine("        <div class='header'>");
            html.AppendLine("            <h1>MIVET - Recordatorio de Cita</h1>");
            html.AppendLine("        </div>");
            html.AppendLine($"        <p>Estimado/a {cita.NombreCliente},</p>");
            html.AppendLine($"        <p>Le recordamos que tiene una cita programada para su mascota <strong>{cita.NombreMascota}</strong>:</p>");
            html.AppendLine("        <div class='cita-info'>");
            html.AppendLine($"            <p><strong>Fecha:</strong> {cita.FechaCita:dd/MM/yyyy}</p>");
            html.AppendLine($"            <p><strong>Hora:</strong> {cita.HoraInicio}</p>");
            html.AppendLine($"            <p><strong>Veterinario:</strong> {cita.NombreVeterinario}</p>");
            html.AppendLine($"            <p><strong>Motivo:</strong> {cita.MotivoConsulta}</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <p>Por favor, llegue 15 minutos antes de su cita.</p>");
            html.AppendLine("        <p>Saludos cordiales,<br>Equipo MIVET</p>");
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private async Task<string> GenerarPlantillaCitaCompletadaAsync(HistorialClinicoCompletoDto historial, FacturaDto factura)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='es'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <title>Cita Completada</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }");
            html.AppendLine("        .container { max-width: 800px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }");
            html.AppendLine("        .header { text-align: center; color: #2c5aa0; margin-bottom: 30px; }");
            html.AppendLine("        .section { margin-bottom: 25px; padding: 15px; background-color: #f8f9fa; border-radius: 5px; }");
            html.AppendLine("        .section-title { font-weight: bold; color: #2c5aa0; margin-bottom: 10px; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class='container'>");
            html.AppendLine("        <div class='header'>");
            html.AppendLine("            <h1>MIVET - Cita Completada</h1>");
            html.AppendLine("        </div>");

            html.AppendLine($"        <p>Estimado/a {historial.HistorialClinico.NombreCliente},</p>");
            html.AppendLine($"        <p>Su cita para <strong>{historial.HistorialClinico.NombreMascota}</strong> ha sido completada exitosamente.</p>");

            // Resumen de la consulta
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <div class='section-title'>Resumen de la Consulta</div>");
            html.AppendLine($"            <p><strong>Fecha:</strong> {historial.HistorialClinico.FechaRegistro:dd/MM/yyyy}</p>");
            html.AppendLine($"            <p><strong>Veterinario:</strong> {historial.HistorialClinico.NombreVeterinario}</p>");
            if (!string.IsNullOrEmpty(historial.HistorialClinico.Diagnostico))
            {
                html.AppendLine($"            <p><strong>Diagnóstico:</strong> {historial.HistorialClinico.Diagnostico}</p>");
            }
            if (!string.IsNullOrEmpty(historial.HistorialClinico.Tratamiento))
            {
                html.AppendLine($"            <p><strong>Tratamiento:</strong> {historial.HistorialClinico.Tratamiento}</p>");
            }
            html.AppendLine("        </div>");

            // Información de facturación
            if (factura != null)
            {
                html.AppendLine("        <div class='section'>");
                html.AppendLine("            <div class='section-title'>Información de Facturación</div>");
                html.AppendLine($"            <p><strong>Factura:</strong> {factura.NumeroFactura}</p>");
                html.AppendLine($"            <p><strong>Total:</strong> ${factura.Total:N2}</p>");
                html.AppendLine($"            <p><strong>Estado:</strong> {factura.Estado}</p>");
                html.AppendLine("        </div>");
            }

            html.AppendLine("        <p>Adjunto encontrará la historia clínica completa de la consulta.</p>");
            html.AppendLine("        <p>Gracias por confiar en nosotros para el cuidado de su mascota.</p>");
            html.AppendLine("        <p>Saludos cordiales,<br>Equipo MIVET</p>");
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        #endregion

        #region Utilidades

        public async Task<bool> ValidarEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
