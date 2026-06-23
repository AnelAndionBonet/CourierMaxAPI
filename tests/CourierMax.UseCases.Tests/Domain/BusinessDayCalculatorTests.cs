using CourierMax.UseCases;
using FluentAssertions;
using Xunit;

public class BusinessDayCalculatorTests
{
    [Fact]
    public void AddBusinessDays_Viernes_mas_1_da_Lunes()
    {
        // 2026-06-19 es viernes
        var resultado = BusinessDayCalculator.AddBusinessDays(new DateTime(2026, 6, 19), 1);
        resultado.Date.Should().Be(new DateTime(2026, 6, 22)); // lunes
    }

    [Fact]
    public void AddBusinessDays_salta_festivo_20Jul()
    {
        // 2026-07-17 viernes; 20-Jul lunes es festivo -> +1 hábil = 21-Jul martes
        var resultado = BusinessDayCalculator.AddBusinessDays(new DateTime(2026, 7, 17), 1);
        resultado.Date.Should().Be(new DateTime(2026, 7, 21));
    }
}
