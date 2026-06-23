namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Distancia y tarifa entre par de ciudades (gral.DistanciasTarifas).
    /// </summary>
    public class DistanciaTarifa : EntidadAuditable<int>
    {
        public int IdCiudadOrigen { get; set; }
        public int IdCiudadDestino { get; set; }
        public int DistanciaKm { get; set; }
        public decimal TarifaDistancia { get; set; }

        // Navegaciones
        public Ciudad CiudadOrigen { get; set; } = null!;
        public Ciudad CiudadDestino { get; set; } = null!;
    }
}
