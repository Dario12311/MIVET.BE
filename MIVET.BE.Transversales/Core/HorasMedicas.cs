using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Core;

public sealed class HorasMedicas
{
    public static readonly HorasMedicas SeisAM = new(1, NombreHoras.SeisAM);
    public static readonly HorasMedicas Seis30AM = new(2, NombreHoras.Seis30AM);
    public static readonly HorasMedicas SieteAM = new(3, NombreHoras.SieteAM);
    public static readonly HorasMedicas Siete30AM = new(4, NombreHoras.Siete30AM);
    public static readonly HorasMedicas OchoAM = new(5, NombreHoras.OchoAM);
    public static readonly HorasMedicas Ocho30AM = new(6, NombreHoras.Ocho30AM);
    public static readonly HorasMedicas NueveAM = new(7, NombreHoras.NueveAM);
    public static readonly HorasMedicas Nueve30AM = new(8, NombreHoras.Nueve30AM);
    public static readonly HorasMedicas DiezAM = new(9, NombreHoras.DiezAM);
    public static readonly HorasMedicas Diez30AM = new(10, NombreHoras.Diez30AM);
    public static readonly HorasMedicas OnceAM = new(11, NombreHoras.OnceAM);
    public static readonly HorasMedicas Once30AM = new(12, NombreHoras.Once30AM);
    public static readonly HorasMedicas DocePM = new(13, NombreHoras.DocePM);
    public static readonly HorasMedicas Doce30PM = new(14, NombreHoras.Doce30PM);
    public static readonly HorasMedicas UnaPM = new(15, NombreHoras.UnaPM);
    public static readonly HorasMedicas Una30PM = new(16, NombreHoras.Una30PM);
    public static readonly HorasMedicas DosPM = new(17, NombreHoras.DosPM);
    public static readonly HorasMedicas Dos30PM = new(18, NombreHoras.Dos30PM);
    public static readonly HorasMedicas TresPM = new(19, NombreHoras.TresPM);
    public static readonly HorasMedicas Tres30PM = new(20, NombreHoras.Tres30PM);
    public static readonly HorasMedicas CuatroPM = new(21, NombreHoras.CuatroPM);
    public static readonly HorasMedicas Cuatro30PM = new(22, NombreHoras.Cuatro30PM);
    public static readonly HorasMedicas CincoPM = new(23, NombreHoras.CincoPM);
    public static readonly HorasMedicas Cinco30PM = new(24, NombreHoras.Cinco30PM);
    public static readonly HorasMedicas SeisPM = new(25, NombreHoras.SeisPM);

    public static class NombreHoras
    {
        public const string SeisAM = "6:00 AM";
        public const string Seis30AM = "6:30 AM";
        public const string SieteAM = "7:00 AM";
        public const string Siete30AM = "7:30 AM";
        public const string OchoAM = "8:00 AM";
        public const string Ocho30AM = "8:30 AM";
        public const string NueveAM = "9:00 AM";
        public const string Nueve30AM = "9:30 AM";
        public const string DiezAM = "10:00 AM";
        public const string Diez30AM = "10:30 AM";
        public const string OnceAM = "11:00 AM";
        public const string Once30AM = "11:30 AM";
        public const string DocePM = "12:00 PM";
        public const string Doce30PM = "12:30 PM";
        public const string UnaPM = "1:00 PM";
        public const string Una30PM = "1:30 PM";
        public const string DosPM = "2:00 PM";
        public const string Dos30PM = "2:30 PM";
        public const string TresPM = "3:00 PM";
        public const string Tres30PM = "3:30 PM";
        public const string CuatroPM = "4:00 PM";
        public const string Cuatro30PM = "4:30 PM";
        public const string CincoPM = "5:00 PM";
        public const string Cinco30PM = "5:30 PM";
        public const string SeisPM = "6:00 PM";
    }

    public int HoraMedicaID { get; }
    public string Code { get; }

    public HorasMedicas(int id, string code)
    {
        HoraMedicaID = id;
        Code = code;
    }

    public HorasMedicas() { }

    public static HorasMedicas[] GetAll()
        => new[] {
            SeisAM,
            Seis30AM,
            SieteAM,
            Siete30AM,
            OchoAM,
            Ocho30AM,
            NueveAM,
            Nueve30AM,
            DiezAM,
            Diez30AM,
            OnceAM,
            Once30AM,
            DocePM,
            Doce30PM,
            UnaPM,
            Una30PM,
            DosPM,
            Dos30PM,
            TresPM,
            Tres30PM,
            CuatroPM,
            Cuatro30PM,
            CincoPM,
            Cinco30PM,
            SeisPM
        };
    public static HorasMedicas GetById(int id)
        => GetAll().First(x => x.HoraMedicaID == id);

    public static HorasMedicas GetByNombre(string nombre)
        => GetAll().First(x => x.Code == nombre);
    public static bool IsValidNombre(string nombre)
        => GetAll().Any(x => x.Code == nombre);
}
