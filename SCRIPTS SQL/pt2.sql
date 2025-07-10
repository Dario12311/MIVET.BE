-- =============================================
-- ENFOQUE SIMPLE Y SEGURO
-- Ejecutar cada comando uno por uno
-- =============================================

-- 1. VERIFICAR QUE LA TABLA EXISTE
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Citas';

-- 2. CREAR ÍNDICES BÁSICOS (uno por uno)
CREATE INDEX IX_Citas_Veterinario_Fecha 
ON dbo.Citas (MedicoVeterinarioNumeroDocumento, FechaCita);

CREATE INDEX IX_Citas_Mascota_Estado 
ON dbo.Citas (MascotaId, EstadoCita);

CREATE INDEX IX_Citas_Fecha_Hora 
ON dbo.Citas (FechaCita, HoraInicio);

-- 3. PROBAR INSERTAR UNA CITA DE EJEMPLO
-- (Asegúrate de que existan las FK primero)
/*
INSERT INTO dbo.Citas 
(MascotaId, MedicoVeterinarioNumeroDocumento, FechaCita, HoraInicio, HoraFin, 
 DuracionMinutos, TipoCita, EstadoCita, CreadoPor, TipoUsuarioCreador)
VALUES 
(1, '1234567890', DATEADD(DAY, 1, GETDATE()), '09:00:00', '09:30:00', 
 30, 1, 1, '0987654321', 1);
*/

-- 4. VERIFICAR QUE LOS ÍNDICES SE CREARON
SELECT 
    i.name AS IndexName,
    i.is_unique,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name = 'Citas'
AND i.name IS NOT NULL
ORDER BY i.name, ic.key_ordinal;