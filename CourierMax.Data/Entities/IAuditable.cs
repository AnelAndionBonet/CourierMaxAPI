namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Define los campos de auditoría comunes a todas las entidades.
    /// </summary>
    public interface IAuditable
    {
        DateTime FechaCreacion { get; set; }
        string CreadoPor { get; set; }
        DateTime? FechaModificacion { get; set; }
        string? ModificadoPor { get; set; }
        DateTime? FechaAnulacion { get; set; }
        string? AnuladoPor { get; set; }
        string? ObservacionEstado { get; set; }
    }
}
