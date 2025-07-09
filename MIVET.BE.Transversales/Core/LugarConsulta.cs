using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Core;

public sealed class LugarConsulta
{

    public static readonly LugarConsulta Consultorio1 = new(1, LugaresConsulta.Consultorio1);
    public static readonly LugarConsulta Consultorio2 = new(2, LugaresConsulta.Consultorio2);
    public static readonly LugarConsulta Consultorio3 = new(3, LugaresConsulta.Consultorio3);
    public static readonly LugarConsulta Consultorio4 = new(4, LugaresConsulta.Consultorio4);

    public static class LugaresConsulta
    {
        public const string Consultorio1 = "CONSULTORIO 1 PRIMER PISO";
        public const string Consultorio2 = "CONSULTORIO 2 PRIMER PISO";
        public const string Consultorio3 = "CONSULTORIO 3 SEGUNDO PISO";
        public const string Consultorio4 = "CONSULTORIO 4 SEGUNDO PISO";
    }
    public int Id { get; }
    public string Name { get; }
    private LugarConsulta(int id, string name)
    {
        Id = id;
        Name = name;
    }
    private LugarConsulta() { }
    public static LugarConsulta[] GetAll() => new[]
    {
    Consultorio1,
    Consultorio2,
    Consultorio3,
    Consultorio4
};
    public static LugarConsulta[] GetAllLugares() => new[]
    {
    Consultorio1,
    Consultorio2,
    Consultorio3,
    Consultorio4
};

    public static LugarConsulta GetById(int id) =>
        GetAll().First(x => x.Id == id);
    public static LugarConsulta GetByName(string name) =>
        GetAll().First(x => x.Name == name);
    public static bool IsValidName(string name) =>
        GetAll().Any(x => x.Name == name);

}
