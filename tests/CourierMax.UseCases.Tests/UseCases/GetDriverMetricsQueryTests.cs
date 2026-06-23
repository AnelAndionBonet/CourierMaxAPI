using CourierMax.UseCases;
using CourierMax.UseCases.Tests.Helpers;
using FluentAssertions;

public class GetDriverMetricsQueryTests
{
    [Fact]
    public async Task ExecuteAsync_Conductor_inexistente_retorna_failure_NotFound()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedCatalogosAsync(ctx);

        var query = new GetDriverMetricsQuery(factory);
        var result = await query.ExecuteAsync(9999);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Conductor_sin_envios_retorna_metricas_en_cero()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx);
        }

        var query = new GetDriverMetricsQuery(factory);
        var result = await query.ExecuteAsync(SeedHelper.ConductorActivoId);

        result.Success.Should().BeTrue();
        result.Value.IdConductor.Should().Be(SeedHelper.ConductorActivoId);
        result.Value.TotalAsignados.Should().Be(0);
        result.Value.Entregados.Should().Be(0);
        result.Value.PesoTotalKg.Should().Be(0m);
    }

    [Fact]
    public async Task ExecuteAsync_Conductor_con_envios_retorna_conteos_y_peso_correctos()
    {
        var factory = new TestDbContextFactory();
        var envio1Id = Guid.NewGuid();
        var envio2Id = Guid.NewGuid();
        var envio3Id = Guid.NewGuid();

        // envio1: Entregado, peso=10
        // envio2: Cancelado, peso=5
        // envio3: EN_TRANSITO, peso=8
        var fechaAsignacion = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);
        var fechaEntrega = new DateTime(2026, 6, 12, 0, 0, 0, DateTimeKind.Utc);

        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx);

            await SeedHelper.SeedEnvioAsync(ctx, envio1Id, SeedHelper.EstadoEntregadoId,
                peso: 10m, idConductor: SeedHelper.ConductorActivoId,
                fechaAsignacion: fechaAsignacion, fechaEntrega: fechaEntrega);

            await SeedHelper.SeedEnvioAsync(ctx, envio2Id, SeedHelper.EstadoCanceladoId,
                peso: 5m, idConductor: SeedHelper.ConductorActivoId);

            await SeedHelper.SeedEnvioAsync(ctx, envio3Id, SeedHelper.EstadoEnTransitoId,
                peso: 8m, idConductor: SeedHelper.ConductorActivoId);
        }

        var query = new GetDriverMetricsQuery(factory);
        var result = await query.ExecuteAsync(SeedHelper.ConductorActivoId);

        result.Success.Should().BeTrue();
        var metrics = result.Value;
        metrics.TotalAsignados.Should().Be(3);
        metrics.Entregados.Should().Be(1);
        metrics.Cancelados.Should().Be(1);
        metrics.EnTransito.Should().Be(1);
        metrics.PesoTotalKg.Should().Be(23m); // 10 + 5 + 8
    }

    [Fact]
    public async Task ExecuteAsync_Conductor_nombre_correcto()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedVehiculoConductorAsync(ctx);
        }

        var query = new GetDriverMetricsQuery(factory);
        var result = await query.ExecuteAsync(SeedHelper.ConductorActivoId);

        result.Success.Should().BeTrue();
        result.Value.Conductor.Should().Be("Carlos Conductor");
    }
}
