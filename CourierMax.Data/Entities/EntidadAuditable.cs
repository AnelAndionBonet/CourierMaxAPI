namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Entidad base con llave primaria genérica y campos de auditoría.
    /// </summary>
    /// <typeparam name="TKey">Tipo de la llave primaria.</typeparam>
    public abstract class EntidadAuditable<TKey> : IAuditable
    {
        public TKey Id { get; set; } = default!;

        public DateTime FechaCreacion { get; set; }
        public string CreadoPor { get; set; } = null!;
        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPor { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string? AnuladoPor { get; set; }
        public string? ObservacionEstado { get; set; }
    }
}
