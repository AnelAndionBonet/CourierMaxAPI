namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Catálogo de ciudades (gral.Ciudades).
    /// </summary>
    public class Ciudad : EntidadAuditable<int>
    {
        public string NombreCiudad { get; set; } = null!;
        public string Codigo { get; set; } = null!;

        public ICollection<Cliente> Clientes { get; } = new List<Cliente>();
    }
}
