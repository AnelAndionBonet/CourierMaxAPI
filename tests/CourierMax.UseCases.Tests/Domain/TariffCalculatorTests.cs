using CourierMax.UseCases;
using FluentAssertions;
using Xunit;

public class TariffCalculatorTests
{
    [Fact]
    public void Calcular_ejemplo_PDF_fragil_express_BogotaMedellin()
    {
        var input = new TarifaInput("EXP", 5m, 12000m, "FRA");
        TariffCalculator.Calcular(input).Should().Be(40950m);
    }

    [Fact]
    public void Calcular_estandar_2kg_sin_recargos()
    {
        var input = new TarifaInput("STD", 2m, 9000m, "DOC");
        TariffCalculator.Calcular(input).Should().Be(17000m); // 8000 base + 0 peso + 9000 dist
    }
}
