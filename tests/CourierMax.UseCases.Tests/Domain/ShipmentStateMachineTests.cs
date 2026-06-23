using CourierMax.UseCases;
using FluentAssertions;
using Xunit;

public class ShipmentStateMachineTests
{
    [Theory]
    [InlineData(EstadoEnvio.Creado, EstadoEnvio.Asignado, true)]
    [InlineData(EstadoEnvio.Asignado, EstadoEnvio.EnTransito, true)]
    [InlineData(EstadoEnvio.EnTransito, EstadoEnvio.Entregado, true)]
    [InlineData(EstadoEnvio.Creado, EstadoEnvio.EnTransito, false)]   // salto inválido
    [InlineData(EstadoEnvio.Entregado, EstadoEnvio.Cancelado, false)] // no se cancela entregado
    [InlineData(EstadoEnvio.Creado, EstadoEnvio.Cancelado, true)]     // cancelar desde cualquiera
    [InlineData(EstadoEnvio.EnTransito, EstadoEnvio.Cancelado, true)]
    public void PuedeTransicionar(EstadoEnvio actual, EstadoEnvio destino, bool esperado)
        => ShipmentStateMachine.PuedeTransicionar(actual, destino).Should().Be(esperado);
}
