using CourierMax.UseCases;
using CourierMax.UseCases.Tests.Helpers;
using FluentAssertions;

public class GetShipmentQueryTests
{
    [Fact]
    public async Task ExecuteAsync_Envio_existente_retorna_success_con_datos()
    {
        var factory = new TestDbContextFactory();
        var envioId = Guid.NewGuid();
        await using (var ctx = factory.CreateDbContext())
        {
            await SeedHelper.SeedCatalogosAsync(ctx);
            await SeedHelper.SeedClientesAsync(ctx);
            await SeedHelper.SeedEnvioAsync(ctx, envioId, SeedHelper.EstadoCreadoId);
        }

        var query = new GetShipmentQuery(factory);
        var result = await query.ExecuteAsync(envioId);

        result.Success.Should().BeTrue();
        result.Value.Id.Should().Be(envioId);
        result.Value.Estado.Should().Be("CREADO");
        result.Value.CodigoRastreo.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Envio_inexistente_retorna_failure_NotFound()
    {
        var factory = new TestDbContextFactory();
        await using (var ctx = factory.CreateDbContext())
            await SeedHelper.SeedCatalogosAsync(ctx);

        var query = new GetShipmentQuery(factory);
        var result = await query.ExecuteAsync(Guid.NewGuid());

        result.Success.Should().BeFalse();
    }
}
