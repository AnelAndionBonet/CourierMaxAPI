namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Catálogo plano de tipos/detalles (gral.TipoDetalles). Aloja unidades de
    /// peso, unidades de volumen, tipos de servicio y tipos de identificación.
    /// </summary>
    public class TipoDetalle : EntidadAuditable<int>
    {
        public string Nombre { get; set; } = null!;
        public string Nomenclatura { get; set; } = null!;
    }
}
