namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Historial de cambios de estado de un envío (core.HistorialEstados).
    /// </summary>
    public class HistorialEstado : EntidadAuditable<long>
    {
        public Guid IdEnvio { get; set; }
        public int? IdEstadoAnterior { get; set; }
        public int IdEstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; }
        public string? Motivo { get; set; }
        public string UsuarioCambio { get; set; } = null!;

        // Navegaciones
        public Envio Envio { get; set; } = null!;
        public Estado? EstadoAnterior { get; set; }
        public Estado EstadoNuevo { get; set; } = null!;
    }
}
