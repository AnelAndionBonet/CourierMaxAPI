-- ============================================================
-- Seed: gral.Estados
-- Idempotente: se puede re-ejecutar sin duplicar (filtra por Nomenclatura).
-- Auditoria: FechaCreacion = SYSUTCDATETIME(), CreadoPor = 'Seed'.
-- El Id lo asigna SQL Server (IDENTITY).
-- ============================================================

INSERT INTO [gral].[Estados] ([Nombre], [Nomenclatura], [FechaCreacion], [CreadoPor])
SELECT v.[Nombre], v.[Nomenclatura], SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('Creado',      'CREADO'),
    ('Asignado',    'ASIGNADO'),
    ('En transito', 'EN_TRANSITO'),
    ('Entransito',  'ENTRANSITO')
) AS v ([Nombre], [Nomenclatura])
WHERE NOT EXISTS (
    SELECT 1 FROM [gral].[Estados] e
    WHERE e.[Nomenclatura] = v.[Nomenclatura]
);

-- Verificacion
SELECT [Id], [Nombre], [Nomenclatura], [FechaCreacion], [CreadoPor]
FROM [gral].[Estados]
ORDER BY [Id];
