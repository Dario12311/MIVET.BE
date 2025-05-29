using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Core;

public sealed class TipoConsulta
{
    public static readonly TipoConsulta General = new(1, Tipos.General);
    public static readonly TipoConsulta Especializada = new(2, Tipos.Especializada);
    public static readonly TipoConsulta Seguimiento = new(3, Tipos.Seguimiento);
    public static readonly TipoConsulta Emergencia = new(4, Tipos.Emergencia);

    public static class Tipos
    {
        public const string General = "General";
        public const string Especializada = "Especializada";
        public const string Seguimiento = "Seguimiento";
        public const string Emergencia = "Emergencia";
    }

    public int TipoConsultaID { get; }
    public string Code { get; }

    private TipoConsulta(int id, string code)
    {
        TipoConsultaID = id;
        Code = code;
    }

    private TipoConsulta() { }

    public static TipoConsulta[] GetAll()
        => new[] { General, Especializada, Seguimiento, Emergencia };

    public static TipoConsulta GetById(int id)
        => GetAll().First(x => x.TipoConsultaID == id);

    public static TipoConsulta GetByCode(string code)
        => GetAll().First(x => x.Code == code);

    public static bool IsValidCode(string code)
        => GetAll().Any(x => x.Code == code);
}
