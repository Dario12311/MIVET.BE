using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Core;

public sealed class Dias
{
    public static readonly Dias Lunes = new(1, NombresDias.Lunes);
    public static readonly Dias Martes = new(2, NombresDias.Martes);
    public static readonly Dias Miercoles = new(3, NombresDias.Miercoles);
    public static readonly Dias Jueves = new(4, NombresDias.Jueves);
    public static readonly Dias Viernes = new(5, NombresDias.Viernes);
    public static readonly Dias Sabado = new(6, NombresDias.Sabado);
    public static readonly Dias Domingo = new(7, NombresDias.Domingo);

    public static class NombresDias
    {
        public const string Lunes = "LUNES";
        public const string Martes = "MARTES";
        public const string Miercoles = "MIÉRCOLES";
        public const string Jueves = "JUEVES";
        public const string Viernes = "VIERNES";
        public const string Sabado = "SÁBADO";
        public const string Domingo = "DOMINGO";
    }

    public static Dias GetByDayOfWeek(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => Lunes,
            DayOfWeek.Tuesday => Martes,
            DayOfWeek.Wednesday => Miercoles,
            DayOfWeek.Thursday => Jueves,
            DayOfWeek.Friday => Viernes,
            DayOfWeek.Saturday => Sabado,
            DayOfWeek.Sunday => Domingo,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Día no válido.")
        };
    }


    public int DiaID { get; }
    public string Code { get; }

    private Dias(int id, string code)
    {
        DiaID = id;
        Code = code;
    }

    private Dias() { }

    public static Dias[] GetAll()
        => new[] {
            Lunes,
            Martes,
            Miercoles,
            Jueves,
            Viernes,
            Sabado,
            Domingo
        };

    public static Dias GetById(int id)
        => GetAll().First(x => x.DiaID == id);

    public static Dias GetByNombre(string nombre)
        => GetAll().First(x => x.Code == nombre);

    public static bool IsValidNombre(string nombre)
        => GetAll().Any(x => x.Code == nombre);

}
