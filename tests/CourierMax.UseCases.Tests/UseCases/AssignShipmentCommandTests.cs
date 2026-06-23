using CourierMax.Data;
using CourierMax.Data.Entities;
using CourierMax.Dtos.Shipments;
using CourierMax.UseCases;
using CourierMax.UseCases.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class AssignShipmentCommandTests
{
    private static AssignShipmentCommand BuildCommand(TestDbContextFactory factory)
        => new(factory);

    [Fact]
    public async Task ExecuteAsync_Envio_CREADO_con_conductor_disponible_success_estado_ASIGNADO()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx, capacidadPeso: 1000m, capacidadVolumen: 100m);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId, peso: 5m);
        }

        var command = BuildCommand(factory);
        var input = new AssignShipmentInput(envioId, "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeTrue();
        result.Value.Estado.Should().Be("ASIGNADO");

        // Verify DB: IdConductor and FechaAsignacion are set
        await using var verify = factory.CreateDbContext();
        var envio = await verify.Envios.FirstAsync(e => e.Id == envioId);
        envio.IdConductor.Should().Be(SeedHelper.ConductorActivoId);
        envio.FechaAsignacion.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_Vehiculo_sin_capacidad_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            // Vehicle with only 1kg capacity; shipment weighs 5kg
            await SeedHelper.SeedVehiculoConductorAsync(ctx, capacidadPeso: 1m, capacidadVolumen: 100m);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId, peso: 5m);
        }

        var command = BuildCommand(factory);
        var input = new AssignShipmentInput(envioId, "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Envio_inexistente_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx);
        }

        var command = BuildCommand(factory);
        var input = new AssignShipmentInput(Guid.NewGuid(), "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Envio_EN_TRANSITO_no_puede_asignarse()
    {
        // ASIGNADO→EN_TRANSITO is valid but assigning EN_TRANSITO→ASIGNADO is not
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoEnTransitoId, peso: 5m);
        }

        var command = BuildCommand(factory);
        var input = new AssignShipmentInput(envioId, "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }
}
