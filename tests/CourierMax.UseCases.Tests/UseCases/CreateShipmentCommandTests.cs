using CourierMax.Data;
using CourierMax.Dtos.Shipments;
using CourierMax.UseCases;
using CourierMax.UseCases.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class CreateShipmentCommandTests
{
    private static CreateShipmentRequest HappyPathRequest(string usuario = "tester") =>
        new(
            IdRemitente: SeedHelper.ClienteRemitente1Id,
            IdDestinatario: SeedHelper.ClienteDestinatario1Id,
            Peso: 5m,
            IdUnidadPeso: SeedHelper.UnidadPesoKgId,
            Largo: 30m,
            Ancho: 20m,
            Alto: 15m,
            IdUnidadVolumen: SeedHelper.UnidadVolumenM3Id,
            IdTipoPaquete: SeedHelper.TipoPaqueteDocId,
            IdTipoServicio: SeedHelper.TipoServicioStdId,
            Usuario: usuario
        );

    [Fact]
    public async Task ExecuteAsync_HappyPath_retorna_success_con_estado_CREADO_y_codigo_CM()
    {
        // Arrange
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedBaselineAsync(ctx);

        var command = new CreateShipmentCommand(factory, new CreateShipmentValidator());

        // Act
        var result = await command.ExecuteAsync(HappyPathRequest());

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Estado.Should().Be("CREADO");
        result.Value.CodigoRastreo.Should().StartWith("CM-");

        // Verify DB: 1 Envio + 1 HistorialEstado
        await using var verify = factory.CreateDbContext();
        var countEnvios = await verify.Envios.CountAsync();
        var countHistorial = await verify.HistorialEstados.CountAsync();
        countEnvios.Should().Be(1);
        countHistorial.Should().Be(1);
    }

    [Fact]
    public async Task ExecuteAsync_HappyPath_Costo_es_el_esperado_por_TariffCalculator()
    {
        // STD + DOC + 5kg: base 8000 + (5-2)*1500 + 12000 = 24500, recargo DOC=0 → 24500
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedBaselineAsync(ctx);

        var command = new CreateShipmentCommand(factory, new CreateShipmentValidator());
        var result = await command.ExecuteAsync(HappyPathRequest());

        result.Success.Should().BeTrue();
        result.Value.Costo.Should().Be(
            TariffCalculator.Calcular(new TarifaInput("STD", 5m, SeedHelper.TarifaBogMedValor, "DOC")));
    }

    [Fact]
    public async Task ExecuteAsync_Remitente_inexistente_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedBaselineAsync(ctx);

        var command = new CreateShipmentCommand(factory, new CreateShipmentValidator());
        var req = HappyPathRequest() with { IdRemitente = 9999 };

        var result = await command.ExecuteAsync(req);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Misma_ciudad_retorna_failure()
    {
        // Put both clients in the same city (Bogotá)
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            // Both clients in Bogotá
            var now = DateTime.UtcNow;
            ctx.Clientes.Add(new CourierMax.Data.Entities.Cliente
            {
                Id = SeedHelper.ClienteRemitente1Id,
                Nombre = "Remitente",
                Telefono = "3001",
                Direccion = "Calle 1",
                IdTipoIdentificacion = SeedHelper.TipoIdentCcId,
                Identificacion = "111",
                IdCiudad = SeedHelper.CiudadBogotaId,
                FechaCreacion = now,
                CreadoPor = "test",
            });
            ctx.Clientes.Add(new CourierMax.Data.Entities.Cliente
            {
                Id = SeedHelper.ClienteDestinatario1Id,
                Nombre = "Destinatario",
                Telefono = "3002",
                Direccion = "Calle 2",
                IdTipoIdentificacion = SeedHelper.TipoIdentCcId,
                Identificacion = "222",
                IdCiudad = SeedHelper.CiudadBogotaId, // same city!
                FechaCreacion = now,
                CreadoPor = "test",
            });
            await ctx.SaveChangesAsync();
            await SeedHelper.SeedTarifasAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx);
        }

        var command = new CreateShipmentCommand(factory, new CreateShipmentValidator());
        var result = await command.ExecuteAsync(HappyPathRequest());

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Sin_tarifa_entre_ciudades_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
        {
            // Seed catalogues and clients but NO tarifas
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            // No tarifas
        }

        var command = new CreateShipmentCommand(factory, new CreateShipmentValidator());
        var result = await command.ExecuteAsync(HappyPathRequest());

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Peso_cero_falla_validacion()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedBaselineAsync(ctx);

        var command = new CreateShipmentCommand(factory, new CreateShipmentValidator());
        var req = HappyPathRequest() with { Peso = 0m };

        var result = await command.ExecuteAsync(req);

        result.Success.Should().BeFalse();
    }
}
