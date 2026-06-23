-- ============================================================
-- Seed: gral.DistanciasTarifas (pares de ciudades del PDF)
-- Bidireccional: se almacena una sola dirección por par.
-- Busca las ciudades por su Código DANE. Idempotente. Ejecutar en CourierMax-COL.
-- ============================================================

INSERT INTO [gral].[DistanciasTarifas]
    ([IdCiudadOrigen],[IdCiudadDestino],[DistanciaKm],[TarifaDistancia],[FechaCreacion],[CreadoPor])
SELECT o.[Id], d.[Id], t.km, t.tarifa, SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('11001','05001', 480, 12000),  -- Bogotá - Medellín
    ('11001','76001', 360,  9000),  -- Bogotá - Cali
    ('11001','08001', 950, 20000),  -- Bogotá - Barranquilla
    ('05001','76001', 310,  8000),  -- Medellín - Cali
    ('05001','08001', 650, 15000),  -- Medellín - Barranquilla
    ('76001','08001', 900, 18000)   -- Cali - Barranquilla
) t(co, cd, km, tarifa)
JOIN [gral].[Ciudades] o ON o.[Codigo] = t.co
JOIN [gral].[Ciudades] d ON d.[Codigo] = t.cd
WHERE NOT EXISTS (
    SELECT 1 FROM [gral].[DistanciasTarifas] x
    WHERE x.[IdCiudadOrigen]=o.[Id] AND x.[IdCiudadDestino]=d.[Id]
);

-- Verificacion
SELECT dt.[Id], o.[NombreCiudad] AS Origen, d.[NombreCiudad] AS Destino, dt.[DistanciaKm], dt.[TarifaDistancia]
FROM [gral].[DistanciasTarifas] dt
JOIN [gral].[Ciudades] o ON o.[Id]=dt.[IdCiudadOrigen]
JOIN [gral].[Ciudades] d ON d.[Id]=dt.[IdCiudadDestino]
ORDER BY dt.[Id];
