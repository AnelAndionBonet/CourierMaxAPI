using CourierMax.Data;
using CourierMax.Dtos.Shipments;
using CourierMax.UseCases;
using CourierMax.UseCases.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

public class ChangeShipmentStateCommandTests
{
    private static ChangeShipmentStateCommand BuildCommand(TestDbContextFactory factory)
        => new(factory);

    [Fact]
    public async Task ExecuteAsync_Salto_invalido_CREADO_a_EN_TRANSITO_retorna_failure_Conflict()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId);
        }

        var command = BuildCommand(factory);
        var input = new ChangeStateInput(envioId, "EN_TRANSITO", null, "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_EN_TRANSITO_a_ENTREGADO_success_y_FechaEntrega_seteada()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoEnTransitoId);
        }

        var command = BuildCommand(factory);
        var input = new ChangeStateInput(envioId, "ENTREGADO", null, "tester");
        var antes = DateTime.UtcNow.AddSeconds(-1);

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeTrue();
        result.Value.Estado.Should().Be("ENTREGADO");

        // Verify FechaEntrega was set in DB
        await using var verify = factory.CreateDbContext();
        var envio = await verify.Envios.FirstAsync(e => e.Id == envioId);
        envio.FechaEntrega.Should().NotBeNull();
        envio.FechaEntrega!.Value.Should().BeAfter(antes);
    }

    [Fact]
    public async Task ExecuteAsync_Cancelar_sin_motivo_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId);
        }

        var command = BuildCommand(factory);
        var input = new ChangeStateInput(envioId, "CANCELADO", Motivo: null, "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Cancelar_con_motivo_corto_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId);
        }

        var command = BuildCommand(factory);
        var input = new ChangeStateInput(envioId, "CANCELADO", Motivo: "abc", "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Envio_inexistente_retorna_NotFound()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedCatalogosAsync(ctx);

        var command = BuildCommand(factory);
        var input = new ChangeStateInput(Guid.NewGuid(), "ASIGNADO", null, "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_CREADO_a_CANCELADO_con_motivo_valido_success()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId);
        }

        var command = BuildCommand(factory);
        var input = new ChangeStateInput(envioId, "CANCELADO", "Cliente solicitó cancelación", "tester");

        var result = await command.ExecuteAsync(input);

        result.Success.Should().BeTrue();
        result.Value.Estado.Should().Be("CANCELADO");
    }
}
