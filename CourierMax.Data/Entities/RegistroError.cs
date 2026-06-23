namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Registro de errores no controlados (gral.RegistroErrores). Tabla de sistema:
    /// no hereda de EntidadAuditable porque no es una entidad de negocio.
    /// </summary>
    public class RegistroError
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; }
        public string Ruta { get; set; } = null!;
        public string Metodo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public string? Detalle { get; set; }
        public string? TraceId { get; set; }
    }
}
