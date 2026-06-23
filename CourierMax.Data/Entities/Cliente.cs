namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Cliente / tercero (core.Clientes).
    /// </summary>
    public class Cliente : EntidadAuditable<int>
    {
        public string Nombre { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Direccion { get; set; } = null!;

        public int IdTipoIdentificacion { get; set; }
        public string Identificacion { get; set; } = null!;

        public int IdCiudad { get; set; }

        // Navegaciones
        public TipoDetalle TipoIdentificacion { get; set; } = null!;
        public Ciudad Ciudad { get; set; } = null!;
    }
}
