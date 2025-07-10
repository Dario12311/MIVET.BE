-- =============================================
-- SCRIPT 1: VERIFICAR VERSION Y CREAR INDICE UNICO
-- Ejecutar este bloque primero
-- =============================================

-- Verificar la versi�n de SQL Server
SELECT @@VERSION;
GO

-- Intentar crear �ndice �nico filtrado (para SQL Server 2008+)
-- Si no funciona, usar el script alternativo m�s abajo
BEGIN TRY
    CREATE UNIQUE INDEX [IX_Citas_NoConflictos] 
    ON [dbo].[Citas] ([MedicoVeterinarioNumeroDocumento], [FechaCita], [HoraInicio], [HoraFin])
    WHERE [EstadoCita] NOT IN (4, 5);
    
    PRINT '�ndice �nico filtrado creado correctamente';
END TRY
BEGIN CATCH
    PRINT 'Error al crear �ndice filtrado: ' + ERROR_MESSAGE();
    PRINT 'Ejecutar el script alternativo para �ndices simples';
END CATCH
GO

-- =============================================
-- SCRIPT 2: INDICE ALTERNATIVO (Si el anterior fall�)
-- Solo ejecutar si el �ndice filtrado no funcion�
-- =============================================

-- Si el �ndice filtrado no funciona, crear uno simple
-- CREATE INDEX [IX_Citas_ConflictoCheck] 
-- ON [dbo].[Citas] ([MedicoVeterinarioNumeroDocumento], [FechaCita], [HoraInicio], [HoraFin]);
-- GO

-- =============================================
-- SCRIPT 3: CREAR TRIGGER DE VALIDACION
-- Ejecutar este bloque separado
-- =============================================

CREATE TRIGGER [dbo].[TR_Citas_ValidarConflictos]
ON [dbo].[Citas]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar conflictos solo para citas activas
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        INNER JOIN [dbo].[Citas] c ON 
            c.MedicoVeterinarioNumeroDocumento = i.MedicoVeterinarioNumeroDocumento
            AND c.FechaCita = i.FechaCita
            AND c.Id != i.Id
            AND c.EstadoCita NOT IN (4, 5) -- No cancelada ni no asisti�
            AND i.EstadoCita NOT IN (4, 5) -- La nueva tampoco
            AND (
                (c.HoraInicio < i.HoraFin AND c.HoraFin > i.HoraInicio)
            )
    )
    BEGIN
        RAISERROR('Conflicto de horario: Ya existe una cita programada para este veterinario en el horario especificado.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Validar que la duraci�n coincida con las horas
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE DATEDIFF(MINUTE, i.HoraInicio, i.HoraFin) != i.DuracionMinutos
    )
    BEGIN
        RAISERROR('La duraci�n especificada no coincide con el rango de horas.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- =============================================
-- SCRIPT 4: CREAR FUNCION AUXILIAR
-- Ejecutar este bloque separado
-- =============================================

CREATE FUNCTION [dbo].[FN_CitasConflicto]
(
    @NumeroDocumento NVARCHAR(20),
    @Fecha DATE,
    @HoraInicio TIME,
    @HoraFin TIME,
    @CitaIdExcluir INT = NULL
)
RETURNS BIT
AS
BEGIN
    DECLARE @TieneConflicto BIT = 0;
    
    IF EXISTS (
        SELECT 1 
        FROM [dbo].[Citas] 
        WHERE MedicoVeterinarioNumeroDocumento = @NumeroDocumento
        AND FechaCita = @Fecha
        AND EstadoCita NOT IN (4, 5)
        AND (HoraInicio < @HoraFin AND HoraFin > @HoraInicio)
        AND (@CitaIdExcluir IS NULL OR Id != @CitaIdExcluir)
    )
    BEGIN
        SET @TieneConflicto = 1;
    END
    
    RETURN @TieneConflicto;
END;
GO

-- =============================================
-- SCRIPT 5: CREAR INDICES ADICIONALES
-- Ejecutar este bloque separado
-- =============================================

-- �ndices para mejorar performance
CREATE INDEX [IX_Citas_EstadoCita_FechaCita]
ON [dbo].[Citas] ([EstadoCita], [FechaCita])
INCLUDE ([MedicoVeterinarioNumeroDocumento], [HoraInicio], [HoraFin]);
GO

CREATE INDEX [IX_Citas_FechaCita_EstadoCita]
ON [dbo].[Citas] ([FechaCita], [EstadoCita])
INCLUDE ([MedicoVeterinarioNumeroDocumento], [MascotaId], [HoraInicio], [HoraFin]);
GO

-- =============================================
-- SCRIPT 6: PROCEDIMIENTO PARA VERIFICAR DISPONIBILIDAD
-- Ejecutar este bloque separado
-- =============================================

CREATE PROCEDURE [dbo].[SP_VerificarDisponibilidad]
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
        FROM [dbo].[Citas] 
        WHERE MedicoVeterinarioNumeroDocumento = @NumeroDocumento
        AND FechaCita = @Fecha
        AND EstadoCita NOT IN (4, 5)
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
            c.EstadoCita,
            c.TipoCita
        FROM [dbo].[Citas] c
        WHERE c.MedicoVeterinarioNumeroDocumento = @NumeroDocumento
        AND c.FechaCita = @Fecha
        AND c.EstadoCita NOT IN (4, 5)
        AND (c.HoraInicio < @HoraFin AND c.HoraFin > @HoraInicio)
        AND (@CitaIdExcluir IS NULL OR c.Id != @CitaIdExcluir);
    END
END;
GO

-- =============================================
-- SCRIPT 7: VERIFICACION FINAL
-- Ejecutar para verificar que todo est� funcionando
-- =============================================

-- Verificar constraints
SELECT 
    TABLE_NAME,
    CONSTRAINT_NAME,
    CONSTRAINT_TYPE
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'Citas';

-- Verificar �ndices creados
SELECT 
    i.name AS IndexName,
    i.is_unique,
    CASE WHEN i.has_filter = 1 THEN 'Yes' ELSE 'No' END AS HasFilter,
    CASE WHEN i.has_filter = 1 THEN i.filter_definition ELSE 'N/A' END AS FilterDefinition
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'Citas'
AND i.name IS NOT NULL
ORDER BY i.name;

-- Verificar triggers
SELECT 
    name AS TriggerName,
    is_disabled
FROM sys.triggers
WHERE parent_id = OBJECT_ID('dbo.Citas');

-- Verificar funciones y procedimientos
SELECT 
    name AS ObjectName,
    type_desc AS ObjectType
FROM sys.objects
WHERE name IN ('FN_CitasConflicto', 'SP_VerificarDisponibilidad');

PRINT 'Verificaci�n completada. Revisa los resultados arriba.';
GO