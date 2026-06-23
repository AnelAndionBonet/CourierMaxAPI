namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Vehículo de transporte (core.Vehiculos).
    /// </summary>
    public class Vehiculo : EntidadAuditable<int>
    {
        public string Placa { get; set; } = null!;
        public decimal CapacidadPesoKg { get; set; }
        public decimal CapacidadVolumenM3 { get; set; }

        // Navegaciones
        public ICollection<Conductor> Conductores { get; } = new List<Conductor>();
    }
}
