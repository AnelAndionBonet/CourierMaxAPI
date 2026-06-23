-- ============================================================
-- Seed: gral.TipoDetalles
-- Idempotente: se puede re-ejecutar sin duplicar (filtra por Nomenclatura).
-- Auditoria: FechaCreacion = SYSUTCDATETIME(), CreadoPor = 'Seed'.
-- El Id lo asigna SQL Server (IDENTITY).
-- ============================================================

INSERT INTO [gral].[TipoDetalles] ([Nombre], [Nomenclatura], [FechaCreacion], [CreadoPor])
SELECT v.[Nombre], v.[Nomenclatura], SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('Cedula de ciudadania', 'CC'),
    ('Documento',            'DOC'),
    ('Paquete frágil',       'FRG'),
    ('Perecedero',           'PER'),
    ('Estándar',             'STD'),
    ('Express',              'EXP'),
    ('Mismo día',            'MSD')
) AS v ([Nombre], [Nomenclatura])
WHERE NOT EXISTS (
    SELECT 1 FROM [gral].[TipoDetalles] t
    WHERE t.[Nomenclatura] = v.[Nomenclatura]
);

-- Verificacion
SELECT [Id], [Nombre], [Nomenclatura], [FechaCreacion], [CreadoPor]
FROM [gral].[TipoDetalles]
ORDER BY [Id];
