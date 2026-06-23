-- ============================================================
-- Correcciones de catálogos para alinear con el PDF
-- Idempotente. Ejecutar en CourierMax-COL.
-- ============================================================

-- gral.Estados: dejar exactamente los 5 estados del PDF
DELETE FROM [gral].[Estados] WHERE [Nomenclatura] = 'ENTRANSITO';

INSERT INTO [gral].[Estados] ([Nombre],[Nomenclatura],[FechaCreacion],[CreadoPor])
SELECT v.n, v.c, SYSUTCDATETIME(), 'Seed'
FROM (VALUES ('Entregado','ENTREGADO'), ('Cancelado','CANCELADO')) v(n,c)
WHERE NOT EXISTS (SELECT 1 FROM [gral].[Estados] e WHERE e.[Nomenclatura]=v.c);

-- gral.TipoDetalles: tipo paquete limpio (4) + unidades de medida
UPDATE [gral].[TipoDetalles] SET [Nombre]='Frágil', [Nomenclatura]='FRA' WHERE [Nomenclatura]='FRG';

INSERT INTO [gral].[TipoDetalles] ([Nombre],[Nomenclatura],[FechaCreacion],[CreadoPor])
SELECT v.n, v.c, SYSUTCDATETIME(), 'Seed'
FROM (VALUES ('Paquete','PAQ'), ('Kilogramo','KG'), ('Metro cúbico','M3')) v(n,c)
WHERE NOT EXISTS (SELECT 1 FROM [gral].[TipoDetalles] t WHERE t.[Nomenclatura]=v.c);

-- Verificacion
SELECT [Id],[Nombre],[Nomenclatura] FROM [gral].[Estados] ORDER BY [Id];
SELECT [Id],[Nombre],[Nomenclatura] FROM [gral].[TipoDetalles] ORDER BY [Id];
