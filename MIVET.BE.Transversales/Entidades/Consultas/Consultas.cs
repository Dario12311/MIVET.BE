using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Entidades;

public class Consultas
{
    public int CitaMedicaID { get; set; }
    public int PacienteID { get; set; }
    public string MedicoID { get; set; }
    public int TipoConsultaID { get; set; }
    public int HorasMedicasID { get; set; }
    public int EstadoCitaID { get; set; }
    [JsonIgnore]
    public int DiaID { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime FechaCita { get; set; }
    public string MotivoConsulta { get; set; }
    public int LugarConsultaID { get; set; }

}
