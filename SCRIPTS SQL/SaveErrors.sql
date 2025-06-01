-- =============================================
-- SOLUCIÓN: ELIMINAR TRIGGER PROBLEMÁTICO
-- Y USAR VALIDACIONES SOLO EN CÓDIGO C#
-- =============================================

-- 1. ELIMINAR EL TRIGGER QUE CAUSA PROBLEMAS CON EF CORE
DROP TRIGGER IF EXISTS [dbo].[TR_Citas_ValidarConflictos];
GO

-- 2. LIMPIAR OBJETOS DUPLICADOS (Ejecutar solo si es necesario)
DROP FUNCTION IF EXISTS [dbo].[FN_CitasConflicto];
DROP PROCEDURE IF EXISTS [dbo].[SP_VerificarDisponibilidad];
GO

-- 3. ELIMINAR ÍNDICES PROBLEMÁTICOS
DROP INDEX IF EXISTS [IX_Citas_Veterinario_Fecha] ON [dbo.Citas];
DROP INDEX IF EXISTS [IX_Citas_NoConflictos] ON [dbo].[Citas];
DROP INDEX IF EXISTS [IX_Citas_EstadoCita_FechaCita] ON [dbo].[Citas];
DROP INDEX IF EXISTS [IX_Citas_FechaCita_EstadoCita] ON [dbo].[Citas];
GO

-- 4. CREAR SOLO ÍNDICES BÁSICOS QUE FUNCIONAN
CREATE INDEX [IX_Citas_Veterinario_Fecha] 
ON [dbo].[Citas] ([MedicoVeterinarioNumeroDocumento], [FechaCita]);

CREATE INDEX [IX_Citas_Mascota_Estado] 
ON [dbo].[Citas] ([MascotaId], [EstadoCita]);

CREATE INDEX [IX_Citas_Fecha_Hora] 
ON [dbo].[Citas] ([FechaCita], [HoraInicio]);

CREATE INDEX [IX_Citas_Estado] 
ON [dbo].[Citas] ([EstadoCita]);
GO

-- 5. VERIFICAR QUE TODO ESTÁ LIMPIO
PRINT 'Limpieza completada. Verificando estado:';

-- Verificar que no hay triggers
SELECT COUNT(*) as TriggersCount FROM sys.triggers WHERE parent_id = OBJECT_ID('dbo.Citas');

-- Verificar índices actuales
SELECT 
    i.name AS IndexName,
    i.is_unique
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'Citas'
AND i.name IS NOT NULL
ORDER BY i.name;

PRINT 'La tabla Citas está lista para usar con Entity Framework';
GO