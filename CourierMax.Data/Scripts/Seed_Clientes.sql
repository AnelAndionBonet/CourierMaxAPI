-- ============================================================
-- Seed: core.Clientes (remitentes/destinatarios para pruebas)
-- Idempotente (filtra por Identificacion). Busca CC en TipoDetalles
-- y la ciudad por su Código DANE. Requiere haber corrido Seed_Ciudades
-- y tener el tipo de identificación 'CC'. Ejecutar en CourierMax-COL.
-- ============================================================

INSERT INTO [core].[Clientes]
    ([Nombre],[Telefono],[Direccion],[IdTipoIdentificacion],[Identificacion],[IdCiudad],[FechaCreacion],[CreadoPor])
SELECT v.nom, v.tel, v.dir, td.[Id], v.ident, ci.[Id], SYSUTCDATETIME(), 'Seed'
FROM (VALUES
    ('Juan Gómez',     '3001112233', 'Cra 7 # 32-10',   '1018201001', '11001'),  -- Bogotá
    ('Ana Torres',     '3102223344', 'Cll 50 # 40-20',  '1018201002', '05001'),  -- Medellín
    ('Pedro Ruiz',     '3203334455', 'Av 6 # 25-30',    '1018201003', '76001'),  -- Cali
    ('Laura Díaz',     '3014445566', 'Cra 43 # 70-15',  '1018201004', '08001'),  -- Barranquilla
    ('Carlos Méndez',  '3505556677', 'Cll 100 # 15-50', '1018201005', '11001')   -- Bogotá
) v(nom, tel, dir, ident, codCiudad)
JOIN [gral].[TipoDetalles] td ON td.[Nomenclatura] = 'CC'
JOIN [gral].[Ciudades] ci ON ci.[Codigo] = v.codCiudad
WHERE NOT EXISTS (
    SELECT 1 FROM [core].[Clientes] c WHERE c.[Identificacion] = v.ident
);

-- Verificacion
SELECT c.[Id], c.[Nombre], c.[Telefono], c.[Identificacion], ci.[NombreCiudad]
FROM [core].[Clientes] c
JOIN [gral].[Ciudades] ci ON ci.[Id] = c.[IdCiudad]
ORDER BY c.[Id];
