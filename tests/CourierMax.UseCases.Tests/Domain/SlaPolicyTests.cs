using CourierMax.UseCases;
using FluentAssertions;

public class SlaPolicyTests
{
    [Fact]
    public void DiasHabiles_STD_retorna_5()
        => SlaPolicy.DiasHabiles("STD").Should().Be(5);

    [Fact]
    public void DiasHabiles_EXP_retorna_2()
        => SlaPolicy.DiasHabiles("EXP").Should().Be(2);

    [Fact]
    public void DiasHabiles_MSD_retorna_0()
        => SlaPolicy.DiasHabiles("MSD").Should().Be(0);

    [Fact]
    public void DiasHabiles_desconocido_retorna_MaxValue()
        => SlaPolicy.DiasHabiles("DESCONOCIDO").Should().Be(int.MaxValue);

    [Fact]
    public void DiasHabiles_cadena_vacia_retorna_MaxValue()
        => SlaPolicy.DiasHabiles("").Should().Be(int.MaxValue);
}
