-- ============================================================
-- Seed: gral.Ciudades
-- Idempotente: se puede re-ejecutar sin duplicar (filtra por Codigo).
-- Auditoria: FechaCreacion = SYSUTCDATETIME(), CreadoPor = 'Seed'.
-- El Id lo asigna SQL Server (IDENTITY). Codigos = DANE.
-- ============================================================

INSERT INTO [gral].[Ciudades] ([NombreCiudad], [Codigo], [FechaCreacion], [CreadoPor])
SELECT v.[NombreCiudad], v.[Codigo], SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('Bogotá',       '11001'),
    ('Medellín',     '05001'),
    ('Cali',         '76001'),
    ('Barranquilla', '08001')
) AS v ([NombreCiudad], [Codigo])
WHERE NOT EXISTS (
    SELECT 1 FROM [gral].[Ciudades] c
    WHERE c.[Codigo] = v.[Codigo]
);

-- Verificacion
SELECT [Id], [NombreCiudad], [Codigo], [FechaCreacion], [CreadoPor]
FROM [gral].[Ciudades]
ORDER BY [Id];
