using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class ConsultasDTO
{
    public int CitaMedicaID { get; set; }
    public string PacienteID { get; set; }
    public string MedicoID { get; set; }
    public string TipoConsultaID { get; set; }
    public string HorasMedicasID { get; set; }
    public string EstadoCitaID { get; set; }
    public string DiaID { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime FechaCita { get; set; }
    public string MotivoConsulta { get; set; }
    public string LugarConsultaID { get; set; }

}
