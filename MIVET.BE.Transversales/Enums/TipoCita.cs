using System;

namespace MIVET.BE.Transversales.Enums
{
    public enum TipoCita
    {
        Normal = 1,
        Operacion = 2,
        Vacunacion = 3,
        Emergencia = 4,
        Control = 5,
        Cirugia = 6
    }

    public enum EstadoCita
    {
        Programada = 1,
        Confirmada = 2,
        EnCurso = 3,
        Completada = 4,
        Cancelada = 5,
        NoAsistio = 6
    }

    public enum TipoUsuarioCreador
    {
        Cliente = 1,
        Administrador = 2,
        Veterinario = 3
    }
}