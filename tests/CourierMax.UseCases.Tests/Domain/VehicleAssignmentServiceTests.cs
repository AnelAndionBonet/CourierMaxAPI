using CourierMax.Dtos.Shipments;
using CourierMax.UseCases;
using FluentAssertions;

public class VehicleAssignmentServiceTests
{
    // Helper: creates a CargaConductorData with spare capacity by default
    private static CargaConductorData Carga(
        int idConductor = 1,
        bool activo = true,
        decimal capPeso = 100m,
        decimal capVol = 10m,
        decimal pesoActual = 0m,
        decimal volActual = 0m)
        => new(IdConductor: idConductor, IdVehiculo: 1, Activo: activo,
               CapacidadPesoKg: capPeso, CapacidadVolumenM3: capVol,
               PesoActualKg: pesoActual, VolumenActualM3: volActual);

    [Fact]
    public void Seleccionar_unico_candidato_con_capacidad_retorna_success()
    {
        var cargas = new[] { Carga(idConductor: 1, capPeso: 50m, capVol: 5m, pesoActual: 0m, volActual: 0m) };

        var resultado = VehicleAssignmentService.Seleccionar(cargas, pesoEnvio: 10m, volumenEnvioM3: 1m);

        resultado.Success.Should().BeTrue();
        resultado.Value.Should().Be(1);
    }

    [Fact]
    public void Seleccionar_dos_candidatos_elige_menor_pesoActual()
    {
        var cargas = new[]
        {
            Carga(idConductor: 1, capPeso: 100m, capVol: 10m, pesoActual: 30m, volActual: 0m),
            Carga(idConductor: 2, capPeso: 100m, capVol: 10m, pesoActual: 10m, volActual: 0m),
        };

        var resultado = VehicleAssignmentService.Seleccionar(cargas, pesoEnvio: 5m, volumenEnvioM3: 0.1m);

        resultado.Success.Should().BeTrue();
        resultado.Value.Should().Be(2); // conductor con menor carga
    }

    [Fact]
    public void Seleccionar_sin_capacidad_de_peso_retorna_failure()
    {
        // Capacity is 10 kg but current load is already 9 kg; envío weighs 5 kg
        var cargas = new[] { Carga(capPeso: 10m, pesoActual: 9m) };

        var resultado = VehicleAssignmentService.Seleccionar(cargas, pesoEnvio: 5m, volumenEnvioM3: 0.1m);

        resultado.Success.Should().BeFalse();
    }

    [Fact]
    public void Seleccionar_sin_capacidad_de_volumen_retorna_failure()
    {
        // Volume capacity is 1 m³ but current usage is already 0.8 m³; shipment needs 0.5 m³
        var cargas = new[] { Carga(capPeso: 100m, capVol: 1m, pesoActual: 0m, volActual: 0.8m) };

        var resultado = VehicleAssignmentService.Seleccionar(cargas, pesoEnvio: 1m, volumenEnvioM3: 0.5m);

        resultado.Success.Should().BeFalse();
    }

    [Fact]
    public void Seleccionar_conductores_inactivos_se_excluyen()
    {
        var cargas = new[]
        {
            Carga(idConductor: 1, activo: false, capPeso: 100m, capVol: 10m),
            Carga(idConductor: 2, activo: false, capPeso: 100m, capVol: 10m),
        };

        var resultado = VehicleAssignmentService.Seleccionar(cargas, pesoEnvio: 1m, volumenEnvioM3: 0.1m);

        resultado.Success.Should().BeFalse();
    }

    [Fact]
    public void Seleccionar_lista_vacia_retorna_failure()
    {
        var resultado = VehicleAssignmentService.Seleccionar(
            Array.Empty<CargaConductorData>(), pesoEnvio: 1m, volumenEnvioM3: 0.1m);

        resultado.Success.Should().BeFalse();
    }
}
