namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Envío (core.Envios). Su llave primaria es un Guid secuencial generado por
    /// NEWSEQUENTIALID() para minimizar la fragmentación del índice clustered.
    /// </summary>
    public class Envio : EntidadAuditable<Guid>
    {
        public int IdRemitente { get; set; }
        public int IdDestinatario { get; set; }

        public string CodigoRastreo { get; set; } = null!;

        public decimal Peso { get; set; }
        public int IdUnidadPeso { get; set; }

        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Alto { get; set; }
        public int IdUnidadVolumen { get; set; }

        public int IdTipoPaquete { get; set; }

        public decimal Costo { get; set; }

        public int IdTipoServicio { get; set; }
        public int IdEstado { get; set; }

        public int? IdConductor { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaEntrega { get; set; }

        // Navegaciones
        public Cliente Remitente { get; set; } = null!;
        public Cliente Destinatario { get; set; } = null!;
        public TipoDetalle UnidadPeso { get; set; } = null!;
        public TipoDetalle UnidadVolumen { get; set; } = null!;
        public TipoDetalle TipoServicio { get; set; } = null!;
        public Estado Estado { get; set; } = null!;
        public TipoDetalle TipoPaquete { get; set; } = null!;
        public Conductor? Conductor { get; set; }
        public ICollection<HistorialEstado> HistorialEstados { get; } = new List<HistorialEstado>();
    }
}
