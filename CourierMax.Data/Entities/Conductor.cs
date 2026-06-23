namespace CourierMax.Data.Entities
{
    /// <summary>
    /// Conductor asignado a un vehículo (core.Conductores).
    /// </summary>
    public class Conductor : EntidadAuditable<int>
    {
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
        public int IdVehiculo { get; set; }

        // Navegaciones
        public Vehiculo Vehiculo { get; set; } = null!;
        public ICollection<Envio> Envios { get; } = new List<Envio>();
    }
}
