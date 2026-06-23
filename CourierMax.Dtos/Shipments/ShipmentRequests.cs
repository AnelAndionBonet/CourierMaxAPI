namespace CourierMax.Dtos.Shipments
{
    /// <summary>Petición para crear un envío (RF-01).</summary>
    public record CreateShipmentRequest(
        int IdRemitente,
        int IdDestinatario,
        decimal Peso,
        int IdUnidadPeso,
        decimal Largo,
        decimal Ancho,
        decimal Alto,
        int IdUnidadVolumen,
        int IdTipoPaquete,
        int IdTipoServicio,
        string Usuario);

    /// <summary>Petición para cambiar el estado de un envío (RF-02).</summary>
    public record ChangeStateRequest(
        string EstadoDestino,
        string? Motivo,
        string Usuario);

    /// <summary>Petición para asignar un conductor a un envío (RF-03).</summary>
    public record AssignShipmentRequest(string Usuario);

    /// <summary>Respuesta estándar de un envío.</summary>
    public record ShipmentResponse(
        Guid Id,
        string CodigoRastreo,
        string Estado,
        decimal Costo);

    /// <summary>Input para cambiar el estado de un envío (caso de uso).</summary>
    public record ChangeStateInput(Guid IdEnvio, string EstadoDestino, string? Motivo, string Usuario);

    /// <summary>Input para asignar un conductor a un envío (caso de uso).</summary>
    public record AssignShipmentInput(Guid IdEnvio, string Usuario);

    /// <summary>Input para consultar envíos atrasados por rango de fechas.</summary>
    public record DateRangeInput(DateTime Desde, DateTime Hasta);
}
