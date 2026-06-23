using CourierMax.Data;
using CourierMax.Data.Entities;

namespace CourierMax.UseCases.Tests.Helpers;

/// <summary>
/// Seeds the InMemory database with catalogues and entities required by use-case tests.
/// All entities have explicit Ids, FechaCreacion = DateTime.UtcNow, CreadoPor = "test".
/// </summary>
public static class SeedHelper
{
    // ── Well-known Id constants ───────────────────────────────────────────────

    // Estados
    public const int EstadoCreadoId = 1;
    public const int EstadoAsignadoId = 2;
    public const int EstadoEnTransitoId = 3;
    public const int EstadoEntregadoId = 4;
    public const int EstadoCanceladoId = 5;

    // TipoDetalles – identificación
    public const int TipoIdentCcId = 10;

    // TipoDetalles – servicio
    public const int TipoServicioStdId = 20;
    public const int TipoServicioExpId = 21;
    public const int TipoServicioMsdId = 22;

    // TipoDetalles – paquete
    public const int TipoPaqueteDocId = 30;
    public const int TipoPaquetePaqId = 31;
    public const int TipoPaqueteFraId = 32;
    public const int TipoPaquetePerSId = 33;

    // TipoDetalles – unidades
    public const int UnidadPesoKgId = 40;
    public const int UnidadVolumenM3Id = 41;

    // Ciudades
    public const int CiudadBogotaId = 1;
    public const int CiudadMedellinId = 2;

    // Clientes
    public const int ClienteRemitente1Id = 1;   // en Bogotá
    public const int ClienteDestinatario1Id = 2; // en Medellín

    // DistanciasTarifas
    public const int TarifaBogMedId = 1;
    public const decimal TarifaBogMedValor = 12000m;

    // Vehículo / Conductor
    public const int VehiculoGrandeId = 1;
    public const int ConductorActivoId = 1;

    // ── Seed methods ─────────────────────────────────────────────────────────

    /// <summary>Seeds all catalogue data (Estados, TipoDetalles, Ciudades). SaveChanges is called.</summary>
    public static async Task SeedCatalogosAsync(CourierMaxContext ctx)
    {
        var now = DateTime.UtcNow;

        ctx.Estados.AddRange(
            Estado(EstadoCreadoId,     "Creado",      "CREADO",      now),
            Estado(EstadoAsignadoId,   "Asignado",    "ASIGNADO",    now),
            Estado(EstadoEnTransitoId, "En Tránsito", "EN_TRANSITO", now),
            Estado(EstadoEntregadoId,  "Entregado",   "ENTREGADO",   now),
            Estado(EstadoCanceladoId,  "Cancelado",   "CANCELADO",   now)
        );

        ctx.TipoDetalles.AddRange(
            TipoDetalle(TipoIdentCcId,     "Cédula de Ciudadanía", "CC",  now),
            TipoDetalle(TipoServicioStdId, "Estándar",             "STD", now),
            TipoDetalle(TipoServicioExpId, "Express",              "EXP", now),
            TipoDetalle(TipoServicioMsdId, "Mismo Día",            "MSD", now),
            TipoDetalle(TipoPaqueteDocId,  "Documento",            "DOC", now),
            TipoDetalle(TipoPaquetePaqId,  "Paquete",              "PAQ", now),
            TipoDetalle(TipoPaqueteFraId,  "Frágil",               "FRA", now),
            TipoDetalle(TipoPaquetePerSId, "Perecedero",           "PER", now),
            TipoDetalle(UnidadPesoKgId,    "Kilogramo",            "KG",  now),
            TipoDetalle(UnidadVolumenM3Id, "Metro Cúbico",         "M3",  now)
        );

        ctx.Ciudades.AddRange(
            Ciudad(CiudadBogotaId,   "Bogotá",   "BOG", now),
            Ciudad(CiudadMedellinId, "Medellín", "MED", now)
        );

        await ctx.SaveChangesAsync();
    }

    /// <summary>Seeds Clientes in distinct cities. Requires catalogues already seeded.</summary>
    public static async Task SeedClientesAsync(CourierMaxContext ctx)
    {
        var now = DateTime.UtcNow;
        ctx.Clientes.AddRange(
            Cliente(ClienteRemitente1Id,  "Juan Remitente",    "111111", CiudadBogotaId,   TipoIdentCcId, now),
            Cliente(ClienteDestinatario1Id, "Pedro Destinatario", "222222", CiudadMedellinId, TipoIdentCcId, now)
        );
        await ctx.SaveChangesAsync();
    }

    /// <summary>Seeds DistanciasTarifas between Bogotá and Medellín.</summary>
    public static async Task SeedTarifasAsync(CourierMaxContext ctx)
    {
        var now = DateTime.UtcNow;
        ctx.DistanciasTarifas.Add(new DistanciaTarifa
        {
            Id = TarifaBogMedId,
            IdCiudadOrigen = CiudadBogotaId,
            IdCiudadDestino = CiudadMedellinId,
            DistanciaKm = 440,
            TarifaDistancia = TarifaBogMedValor,
            FechaCreacion = now,
            CreadoPor = "test",
        });
        await ctx.SaveChangesAsync();
    }

    /// <summary>Seeds a Vehiculo and one active Conductor with large capacity.</summary>
    public static async Task SeedVehiculoConductorAsync(CourierMaxContext ctx,
        decimal capacidadPeso = 1000m, decimal capacidadVolumen = 100m, bool activo = true)
    {
        var now = DateTime.UtcNow;
        ctx.Vehiculos.Add(new Vehiculo
        {
            Id = VehiculoGrandeId,
            Placa = "ABC-123",
            CapacidadPesoKg = capacidadPeso,
            CapacidadVolumenM3 = capacidadVolumen,
            FechaCreacion = now,
            CreadoPor = "test",
        });
        ctx.Conductores.Add(new Conductor
        {
            Id = ConductorActivoId,
            Nombre = "Carlos Conductor",
            Activo = activo,
            IdVehiculo = VehiculoGrandeId,
            FechaCreacion = now,
            CreadoPor = "test",
        });
        await ctx.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds full baseline data: catalogues, clientes, tarifas, vehículo/conductor.
    /// </summary>
    public static async Task SeedBaselineAsync(CourierMaxContext ctx)
    {
        await SeedCatalogosAsync(ctx);
        await SeedClientesAsync(ctx);
        await SeedTarifasAsync(ctx);
        await SeedVehiculoConductorAsync(ctx);
    }

    /// <summary>
    /// Creates and persists an Envio with an explicit Guid, explicit Estado, and all required fields.
    /// Returns the saved Envio (Id will be the explicit Guid on InMemory).
    /// </summary>
    public static async Task<Envio> SeedEnvioAsync(CourierMaxContext ctx,
        Guid id,
        int idEstado,
        int idTipoServicio = TipoServicioStdId,
        decimal peso = 2m,
        int? idConductor = null,
        DateTime? fechaAsignacion = null,
        DateTime? fechaEntrega = null,
        DateTime? fechaCreacion = null)
    {
        var now = fechaCreacion ?? DateTime.UtcNow;
        var envio = new Envio
        {
            Id = id,
            IdRemitente = ClienteRemitente1Id,
            IdDestinatario = ClienteDestinatario1Id,
            CodigoRastreo = "CM-" + id.ToString("N")[..8].ToUpper(),
            Peso = peso,
            IdUnidadPeso = UnidadPesoKgId,
            Largo = 10m,
            Ancho = 10m,
            Alto = 10m,
            IdUnidadVolumen = UnidadVolumenM3Id,
            IdTipoPaquete = TipoPaqueteDocId,
            Costo = 17000m,
            IdTipoServicio = idTipoServicio,
            IdEstado = idEstado,
            IdConductor = idConductor,
            FechaAsignacion = fechaAsignacion,
            FechaEntrega = fechaEntrega,
            FechaCreacion = now,
            CreadoPor = "test",
        };
        ctx.Envios.Add(envio);
        await ctx.SaveChangesAsync();
        return envio;
    }

    // ── Private factory helpers ───────────────────────────────────────────────

    private static Estado Estado(int id, string nombre, string nom, DateTime now) => new()
    {
        Id = id, Nombre = nombre, Nomenclatura = nom, FechaCreacion = now, CreadoPor = "test",
    };

    private static TipoDetalle TipoDetalle(int id, string nombre, string nom, DateTime now) => new()
    {
        Id = id, Nombre = nombre, Nomenclatura = nom, FechaCreacion = now, CreadoPor = "test",
    };

    private static Ciudad Ciudad(int id, string nombre, string codigo, DateTime now) => new()
    {
        Id = id, NombreCiudad = nombre, Codigo = codigo, FechaCreacion = now, CreadoPor = "test",
    };

    private static Cliente Cliente(int id, string nombre, string identificacion, int idCiudad, int idTipoIdent, DateTime now) => new()
    {
        Id = id,
        Nombre = nombre,
        Telefono = "3001234567",
        Direccion = "Calle Test 1",
        IdTipoIdentificacion = idTipoIdent,
        Identificacion = identificacion,
        IdCiudad = idCiudad,
        FechaCreacion = now,
        CreadoPor = "test",
    };
}
