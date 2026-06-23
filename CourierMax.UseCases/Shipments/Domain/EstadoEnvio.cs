namespace CourierMax.UseCases;

public enum EstadoEnvio
{
    Creado = 1,
    Asignado = 2,
    EnTransito = 3,
    Entregado = 4,
    Cancelado = 5,
}

public static class EstadoEnvioExtensions
{
    public static string Nomenclatura(this EstadoEnvio estado) => estado switch
    {
        EstadoEnvio.Creado => "CREADO",
        EstadoEnvio.Asignado => "ASIGNADO",
        EstadoEnvio.EnTransito => "EN_TRANSITO",
        EstadoEnvio.Entregado => "ENTREGADO",
        EstadoEnvio.Cancelado => "CANCELADO",
        _ => throw new ArgumentOutOfRangeException(nameof(estado)),
    };

    /// <summary>Convierte una nomenclatura a su EstadoEnvio. Devuelve null si no es válida.</summary>
    public static EstadoEnvio? DesdeNomenclatura(string? nomenclatura) => nomenclatura switch
    {
        "CREADO" => EstadoEnvio.Creado,
        "ASIGNADO" => EstadoEnvio.Asignado,
        "EN_TRANSITO" => EstadoEnvio.EnTransito,
        "ENTREGADO" => EstadoEnvio.Entregado,
        "CANCELADO" => EstadoEnvio.Cancelado,
        _ => null,
    };
}
