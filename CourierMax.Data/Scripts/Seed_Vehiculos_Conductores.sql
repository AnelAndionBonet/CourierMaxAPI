-- ============================================================
-- Seed: core.Vehiculos + core.Conductores (datos de referencia del PDF)
-- Idempotente. Conductor 1:1 con vehículo (por Placa). Ejecutar en CourierMax-COL.
-- ============================================================

-- Vehículos (Placa, CapacidadPesoKg, CapacidadVolumenM3)
INSERT INTO [core].[Vehiculos] ([Placa],[CapacidadPesoKg],[CapacidadVolumenM3],[FechaCreacion],[CreadoPor])
SELECT v.p, v.peso, v.vol, SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('ABC-123', 500, 10),
    ('DEF-456', 300, 6),
    ('GHI-789', 800, 15)
) v(p, peso, vol)
WHERE NOT EXISTS (SELECT 1 FROM [core].[Vehiculos] x WHERE x.[Placa] = v.p);

-- Conductores (activos), asignados 1:1 al vehículo por su Placa
INSERT INTO [core].[Conductores] ([Nombre],[Activo],[IdVehiculo],[FechaCreacion],[CreadoPor])
SELECT c.n, 1, ve.[Id], SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('Juan Pérez',  'ABC-123'),
    ('María López', 'DEF-456'),
    ('Carlos Ruiz', 'GHI-789')
) c(n, placa)
JOIN [core].[Vehiculos] ve ON ve.[Placa] = c.placa
WHERE NOT EXISTS (SELECT 1 FROM [core].[Conductores] x WHERE x.[IdVehiculo] = ve.[Id]);

-- Verificacion
SELECT co.[Id], co.[Nombre], co.[Activo], ve.[Placa], ve.[CapacidadPesoKg], ve.[CapacidadVolumenM3]
FROM [core].[Conductores] co JOIN [core].[Vehiculos] ve ON ve.[Id]=co.[IdVehiculo]
ORDER BY co.[Id];
