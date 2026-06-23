namespace CourierMax.Dtos.Shipments
{
    /// <summary>Carga actual de un conductor/vehículo (Infraestructura → Core).</summary>
    public record CargaConductorData(
        int IdConductor,
        int IdVehiculo,
        bool Activo,
        decimal CapacidadPesoKg,
        decimal CapacidadVolumenM3,
        decimal PesoActualKg,
        decimal VolumenActualM3);

    /// <summary>Envío atrasado respecto al SLA (Core → API).</summary>
    public record DelayedShipmentDto(
        Guid Id,
        string CodigoRastreo,
        string Servicio,
        string Estado,
        DateTime FechaCreacion,
        int DiasHabilesTranscurridos,
        int SlaDias);
}
