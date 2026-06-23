namespace CourierMax.UseCases;

public static class ShipmentStateMachine
{
    private static readonly Dictionary<EstadoEnvio, EstadoEnvio[]> Transiciones = new()
    {
        [EstadoEnvio.Creado]     = new[] { EstadoEnvio.Asignado, EstadoEnvio.Cancelado },
        [EstadoEnvio.Asignado]   = new[] { EstadoEnvio.EnTransito, EstadoEnvio.Cancelado },
        [EstadoEnvio.EnTransito] = new[] { EstadoEnvio.Entregado, EstadoEnvio.Cancelado },
        [EstadoEnvio.Entregado]  = Array.Empty<EstadoEnvio>(),
        [EstadoEnvio.Cancelado]  = Array.Empty<EstadoEnvio>(),
    };

    public static IReadOnlyCollection<EstadoEnvio> TransicionesValidas(EstadoEnvio actual)
        => Transiciones[actual];

    public static bool PuedeTransicionar(EstadoEnvio actual, EstadoEnvio destino)
        => Transiciones[actual].Contains(destino);
}
