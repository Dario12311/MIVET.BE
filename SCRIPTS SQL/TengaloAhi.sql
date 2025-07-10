-- =============================================
-- VISTAS PARA CONSULTAS OPTIMIZADAS
-- =============================================

-- Vista para citas con información completa
CREATE OR ALTER VIEW dbo.VW_CitasCompletas AS
SELECT 
    c.Id,
    c.FechaCita,
    c.HoraInicio,
    c.HoraFin,
    c.DuracionMinutos,
    c.TipoCita,
    c.EstadoCita,
    c.MotivoConsulta,
    c.Observaciones,
    c.FechaCreacion,
    c.FechaModificacion,
    c.CreadoPor,
    c.TipoUsuarioCreador,
    
    -- Información de la mascota
    m.Id AS MascotaId,
    m.Nombre AS NombreMascota,
    m.Especie,
    m.Raza,
    m.Edad AS EdadMascota,
    m.Genero AS GeneroMascota,
    
    -- Información del veterinario
    mv.NumeroDocumento AS VeterinarioNumeroDocumento,
    mv.Nombre AS NombreVeterinario,
    mv.Especialidad,
    mv.Telefono AS TelefonoVeterinario,
    mv.CorreoElectronico AS EmailVeterinario,
    
    -- Información del cliente
    pc.NumeroDocumento AS ClienteNumeroDocumento,
    pc.PrimerNombre + ' ' + pc.PrimerApellido AS NombreCliente,
    pc.Telefono AS TelefonoCliente,
    pc.CorreoElectronico AS EmailCliente,
    
    -- Campos calculados
    CASE 
        WHEN c.EstadoCita = 5 AND c.FechaCita < CAST(GETDATE() AS DATE) THEN 1
        ELSE 0
    END AS EsCitaVencida,
    
    CASE 
        WHEN c.FechaCita = CAST(GETDATE() AS DATE) THEN 1
        ELSE 0
    END AS EsCitaHoy,
    
    DATEDIFF(MINUTE, c.HoraInicio, c.HoraFin) AS DuracionCalculada

FROM dbo.Citas c
INNER JOIN dbo.Mascota m ON c.MascotaId = m.Id
INNER JOIN dbo.MedicoVeterinario mv ON c.MedicoVeterinarioNumeroDocumento = mv.NumeroDocumento
INNER JOIN dbo.PersonaCliente pc ON m.NumeroDocumentoCliente = pc.NumeroDocumento
WHERE m.Estado = 'A' AND mv.Estado = 'A' AND pc.Estado = 'A';

-- Vista para disponibilidad diaria por veterinario
CREATE OR ALTER VIEW dbo.VW_DisponibilidadDiaria AS
WITH HorarioSlots AS (
    SELECT 
        hv.MedicoVeterinarioNumeroDocumento,
        hv.DiaSemana,
        hv.HoraInicio,
        hv.HoraFin,
        mv.Nombre AS NombreVeterinario,
        mv.Especialidad,
        -- Generar slots de 15 minutos
        DATEADD(MINUTE, (ROW_NUMBER() OVER (PARTITION BY hv.Id ORDER BY hv.Id) - 1) * 15, hv.HoraInicio) AS SlotInicio,
        DATEADD(MINUTE, ROW_NUMBER() OVER (PARTITION BY hv.Id ORDER BY hv.Id) * 15, hv.HoraInicio) AS SlotFin
    FROM dbo.HorarioVeterinarios hv
    INNER JOIN dbo.MedicoVeterinario mv ON hv.MedicoVeterinarioNumeroDocumento = mv.NumeroDocumento
    CROSS JOIN (SELECT TOP 32 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Slot FROM sys.objects) slots
    WHERE hv.EsActivo = 1 
    AND mv.Estado = 'A'
    AND DATEADD(MINUTE, (slots.Slot - 1) * 15, hv.HoraInicio) < hv.HoraFin
)
SELECT 
    hs.MedicoVeterinarioNumeroDocumento,
    hs.NombreVeterinario,
    hs.Especialidad,
    hs.DiaSemana,
    hs.SlotInicio,
    hs.SlotFin,
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM dbo.Citas c 
            WHERE c.MedicoVeterinarioNumeroDocumento = hs.MedicoVeterinarioNumeroDocumento
            AND DATEPART(WEEKDAY, c.FechaCita) = hs.DiaSemana + 1
            AND c.HoraInicio < hs.SlotFin 
            AND c.HoraFin > hs.SlotInicio
            AND c.EstadoCita NOT IN (4, 5) -- No cancelada ni no asistió
        ) THEN 0 
        ELSE 1 
    END AS EsDisponible
FROM HorarioSlots hs;

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- Procedimiento para obtener disponibilidad de un veterinario en una fecha
CREATE OR ALTER PROCEDURE dbo.SP_ObtenerDisponibilidadVeterinario
    @NumeroDocumento NVARCHAR(20),
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DiaSemana INT = DATEPART(WEEKDAY, @Fecha) - 1;
    
    -- Obtener horario configurado
    SELECT 
        hv.MedicoVeterinarioNumeroDocumento,
        mv.Nombre AS NombreVeterinario,
        hv.DiaSemana,
        hv.HoraInicio,
        hv.HoraFin,
        @Fecha AS Fecha
    FROM dbo.HorarioVeterinarios hv
    INNER JOIN dbo.MedicoVeterinario mv ON hv.MedicoVeterinarioNumeroDocumento = mv.NumeroDocumento
    WHERE hv.MedicoVeterinarioNumeroDocumento = @NumeroDocumento
    AND hv.DiaSemana = @DiaSemana
    AND hv.EsActivo = 1
    AND mv.Estado = 'A';
    
    -- Obtener citas existentes
    SELECT 
        c.Id,
        c.HoraInicio,
        c.HoraFin,
        c.TipoCita,
        c.EstadoCita,
        m.Nombre AS NombreMascota,
        pc.PrimerNombre + ' ' + pc.PrimerApellido AS NombreCliente
    FROM dbo.Citas c
    INNER JOIN dbo.Mascota m ON c.MascotaId = m.Id
    INNER JOIN dbo.PersonaCliente pc ON m.NumeroDocumentoCliente = pc.NumeroDocumento
    WHERE c.MedicoVeterinarioNumeroDocumento = @NumeroDocumento
    AND c.FechaCita = @Fecha
    AND c.EstadoCita NOT IN (4, 5) -- No cancelada ni no asistió
    ORDER BY c.HoraInicio;
END;

-- Procedimiento para verificar conflictos de horario
CREATE OR ALTER PROCEDURE dbo.SP_VerificarConflictoHorario
    @NumeroDocumento NVARCHAR(20),
    @Fecha DATE,
    @HoraInicio TIME,
    @HoraFin TIME,
    @CitaIdExcluir INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TieneConflicto BIT = 0;
    
    -- Verificar conflictos con otras citas
    IF EXISTS (
        SELECT 1 
        FROM dbo.Citas 
        WHERE MedicoVeterinarioNumeroDocumento = @NumeroDocumento
        AND FechaCita = @Fecha
        AND EstadoCita NOT IN (4, 5) -- No cancelada ni no asistió
        AND (HoraInicio < @HoraFin AND HoraFin > @HoraInicio)
        AND (@CitaIdExcluir IS NULL OR Id != @CitaIdExcluir)
    )
    BEGIN
        SET @TieneConflicto = 1;
    END
    
    SELECT @TieneConflicto AS TieneConflicto;
    
    -- Devolver citas en conflicto si las hay
    IF @TieneConflicto = 1
    BEGIN
        SELECT 
            c.Id,
            c.HoraInicio,
            c.HoraFin,
            m.Nombre AS NombreMascota,
            pc.PrimerNombre + ' ' + pc.PrimerApellido AS NombreCliente
        FROM dbo.Citas c
        INNER JOIN dbo.Mascota m ON c.MascotaId = m.Id
        INNER JOIN dbo.PersonaCliente pc ON m.NumeroDocumentoCliente = pc.NumeroDocumento
        WHERE c.MedicoVeterinarioNumeroDocumento = @NumeroDocumento
        AND c.FechaCita = @Fecha
        AND c.EstadoCita NOT IN (4, 5)
        AND (c.HoraInicio < @HoraFin AND c.HoraFin > @HoraInicio)
        AND (@CitaIdExcluir IS NULL OR c.Id != @CitaIdExcluir);
    END
END;

-- Procedimiento para procesar citas vencidas
CREATE OR ALTER PROCEDURE dbo.SP_ProcesarCitasVencidas
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @CitasProcesadas INT = 0;
    
    -- Actualizar citas programadas que ya pasaron su hora
    UPDATE dbo.Citas 
    SET EstadoCita = 6, -- NoAsistio
        FechaModificacion = @FechaActual
    WHERE EstadoCita = 1 -- Programada
    AND CAST(FechaCita AS DATETIME) + CAST(HoraFin AS DATETIME) < @FechaActual;
    
    SET @CitasProcesadas = @@ROWCOUNT;
    
    SELECT @CitasProcesadas AS CitasProcesadas;
END;

-- =============================================
-- FUNCIONES ÚTILES
-- =============================================

-- Función para calcular slots disponibles por día
CREATE OR ALTER FUNCTION dbo.FN_CalcularSlotsDisponibles(
    @NumeroDocumento NVARCHAR(20),
    @Fecha DATE
)
RETURNS TABLE
AS
RETURN
(
    WITH HorarioBase AS (
        SELECT 
            hv.HoraInicio,
            hv.HoraFin
        FROM dbo.HorarioVeterinarios hv
        WHERE hv.MedicoVeterinarioNumeroDocumento = @NumeroDocumento
        AND hv.DiaSemana = DATEPART(WEEKDAY, @Fecha) - 1
        AND hv.EsActivo = 1
    ),
    Slots AS (
        SELECT 
            DATEADD(MINUTE, (ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1) * 15, hb.HoraInicio) AS SlotInicio,
            DATEADD(MINUTE, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) * 15, hb.HoraInicio) AS SlotFin
        FROM HorarioBase hb
        CROSS JOIN (SELECT TOP 32 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Slot FROM sys.objects) slots
        WHERE DATEADD(MINUTE, (slots.Slot - 1) * 15, hb.HoraInicio) < hb.HoraFin
    )
    SELECT 
        SlotInicio,
        SlotFin,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM dbo.Citas c 
                WHERE c.MedicoVeterinarioNumeroDocumento = @NumeroDocumento
                AND c.FechaCita = @Fecha
                AND c.HoraInicio < s.SlotFin 
                AND c.HoraFin > s.SlotInicio
                AND c.EstadoCita NOT IN (4, 5)
            ) THEN 0 
            ELSE 1 
        END AS EsDisponible
    FROM Slots s
);

-- =============================================
-- TRIGGERS
-- =============================================

-- Trigger para auditoría de cambios en citas
CREATE OR ALTER TRIGGER dbo.TR_Citas_Auditoria
ON dbo.Citas
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Log de cambios (requiere tabla de auditoría)
    -- Aquí puedes implementar logging detallado si es necesario
    
    -- Ejemplo básico de validación en trigger
    IF UPDATE(HoraInicio) OR UPDATE(HoraFin) OR UPDATE(DuracionMinutos)
    BEGIN
        -- Verificar que la duración calculada coincida con la duración especificada
        IF EXISTS (
            SELECT 1 FROM inserted i
            WHERE DATEDIFF(MINUTE, i.HoraInicio, i.HoraFin) != i.DuracionMinutos
        )
        BEGIN
            RAISERROR('La duración especificada no coincide con el rango de horas', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
    END
END;

-- =============================================
-- ÍNDICES ADICIONALES PARA PERFORMANCE
-- =============================================

-- Índice para búsquedas por estado y fecha
CREATE NONCLUSTERED INDEX IX_Citas_EstadoCita_FechaCita
ON dbo.Citas (EstadoCita, FechaCita)
INCLUDE (MedicoVeterinarioNumeroDocumento, HoraInicio, HoraFin);

-- Índice para búsquedas de citas del día
CREATE NONCLUSTERED INDEX IX_Citas_FechaCita_EstadoCita
ON dbo.Citas (FechaCita, EstadoCita)
INCLUDE (MedicoVeterinarioNumeroDocumento, MascotaId, HoraInicio, HoraFin);

-- =============================================
-- DATOS DE EJEMPLO (PARA DESARROLLO)
-- =============================================

-- Insertar tipos de cita de ejemplo si no existen
-- (Esto depende de cómo manejes los enums en tu aplicación)

-- Script para limpiar datos de prueba
/*
DELETE FROM dbo.Citas WHERE Id > 0;
DBCC CHECKIDENT ('dbo.Citas', RESEED, 0);
*/

-- =============================================
-- CONSULTAS ÚTILES PARA ADMINISTRACIÓN
-- =============================================

-- Ver citas del día actual
-- SELECT * FROM dbo.VW_CitasCompletas WHERE FechaCita = CAST(GETDATE() AS DATE);

-- Ver disponibilidad de todos los veterinarios para hoy
-- SELECT * FROM dbo.VW_DisponibilidadDiaria WHERE DiaSemana = DATEPART(WEEKDAY, GETDATE()) - 1;

-- Estadísticas de citas por veterinario
/*
SELECT 
    VeterinarioNumeroDocumento,
    NombreVeterinario,
    COUNT(*) AS TotalCitas,
    COUNT(CASE WHEN EstadoCita = 4 THEN 1 END) AS CitasCompletadas,
    COUNT(CASE WHEN EstadoCita = 5 THEN 1 END) AS CitasCanceladas,
    COUNT(CASE WHEN EstadoCita = 6 THEN 1 END) AS CitasNoAsistio
FROM dbo.VW_CitasCompletas
WHERE FechaCita >= DATEADD(MONTH, -1, GETDATE())
GROUP BY VeterinarioNumeroDocumento, NombreVeterinario
ORDER BY TotalCitas DESC;
*/