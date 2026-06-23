using CourierMax.Dtos.Shipments;
using CourierMax.UseCases;
using CourierMax.UseCases.Tests.Helpers;
using FluentAssertions;

public class GetDelayedShipmentsQueryTests
{
    [Fact]
    public async Task ExecuteAsync_Rango_invalido_Hasta_menor_Desde_retorna_failure()
    {
        var factory = new TestDbContextFactory();
        var query = new GetDelayedShipmentsQuery(factory);

        var hoy = DateTime.UtcNow;
        var input = new DateRangeInput(Desde: hoy.AddDays(1), Hasta: hoy); // invalid range

        var result = await query.ExecuteAsync(input);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Envio_Express_con_SLA_vencido_aparece_en_lista()
    {
        // EXP SLA = 2 business days.
        // Create shipment with FechaCreacion 10 business days ago → should be delayed.
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();

        // Go back 10 business days from today (2026-06-21): we use a safe past date.
        // BusinessDayCalculator.BusinessDaysBetween(past, now) > 2 must hold.
        // Use 2026-06-01 (Monday) → plenty of business days before 2026-06-21.
        var fechaCreacion = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(
                ctx,
                envioId,
                idEstado: SeedHelper.EstadoAsignadoId, // not delivered, not cancelled
                idTipoServicio: SeedHelper.TipoServicioExpId,
                fechaCreacion: fechaCreacion);
        }

        var query = new GetDelayedShipmentsQuery(factory);
        // Range covers the shipment
        var input = new DateRangeInput(
            Desde: fechaCreacion.AddDays(-1),
            Hasta: DateTime.UtcNow.AddDays(1));

        var result = await query.ExecuteAsync(input);

        result.Success.Should().BeTrue();
        result.Value.Should().ContainSingle(d => d.Id == envioId);
        result.Value[0].DiasHabilesTranscurridos.Should().BeGreaterThan(2);
        result.Value[0].SlaDias.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_Envio_entregado_no_aparece_en_lista()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        var fechaCreacion = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(
                ctx,
                envioId,
                idEstado: SeedHelper.EstadoEntregadoId, // delivered → excluded
                idTipoServicio: SeedHelper.TipoServicioExpId,
                fechaCreacion: fechaCreacion);
        }

        var query = new GetDelayedShipmentsQuery(factory);
        var input = new DateRangeInput(
            Desde: fechaCreacion.AddDays(-1),
            Hasta: DateTime.UtcNow.AddDays(1));

        var result = await query.ExecuteAsync(input);

        result.Success.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
