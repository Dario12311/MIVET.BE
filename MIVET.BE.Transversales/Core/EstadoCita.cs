using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVET.BE.Transversales.Core;

public sealed class EstadoCita
{
    public static readonly EstadoCita Pendiente = new(1, EstadoCitas.Pendiente);
    public static readonly EstadoCita Completada = new(2, EstadoCitas.Completada);
    public static readonly EstadoCita Ocupado = new(3, EstadoCitas.EnCurso);
    public static readonly EstadoCita Cancelada = new(4, EstadoCitas.Cancelada);
    public static readonly EstadoCita NoAsistio = new(5, EstadoCitas.NoAsistio);

    public static class EstadoCitas
    {
        public const string Pendiente = "PENDIENTE";
        public const string Completada = "COMPLETADA";
        public const string EnCurso = "EN CURSO";
        public const string Cancelada = "CANCELADA";
        public const string NoAsistio = "NO ASISTIO";
    }

    public int EstadoCitaID { get; }

    public string Code { get; }

    private EstadoCita(int EstadoCitaId, string code)
    {
        EstadoCitaID = EstadoCitaId;
        Code = code;
    }

    private EstadoCita() { }

    public static EstadoCita[] GetAll()
        => new[] {
        Pendiente,
        Ocupado,
        Completada,
        Cancelada,
        NoAsistio
        };

    public static EstadoCita GetById(int id)
        => GetAll().First(x => x.EstadoCitaID == id);

    public static EstadoCita GetByCode(string code)
        => GetAll().First(x => x.Code == code);

    public static bool IsValidCode(string code)
        => GetAll().Any(x => x.Code == code);
}
