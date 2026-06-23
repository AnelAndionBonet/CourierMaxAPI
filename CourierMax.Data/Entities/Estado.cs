namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Catálogo de estados (gral.Estados).
    /// </summary>
    public class Estado : EntidadAuditable<int>
    {
        public string Nombre { get; set; } = null!;
        public string Nomenclatura { get; set; } = null!;

        public ICollection<Envio> Envios { get; } = new List<Envio>();
    }
}
