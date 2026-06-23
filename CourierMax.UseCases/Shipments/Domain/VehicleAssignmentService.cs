using CourierMax.Dtos.Shipments;
using ROP;

namespace CourierMax.UseCases;

/// <summary>
/// Servicio de dominio para seleccionar el conductor óptimo para un envío (RN-01).
/// Filtra conductores activos con capacidad suficiente (peso Y volumen) y elige el de
/// menor carga actual de peso para balancear la flota.
/// </summary>
public static class VehicleAssignmentService
{
    public static Result<int> Seleccionar(
        IReadOnlyList<CargaConductorData> cargas,
        decimal pesoEnvio,
        decimal volumenEnvioM3)
    {
        var candidato = cargas
            .Where(c => c.Activo
                && (c.CapacidadPesoKg - c.PesoActualKg) >= pesoEnvio
                && (c.CapacidadVolumenM3 - c.VolumenActualM3) >= volumenEnvioM3)
            .OrderBy(c => c.PesoActualKg)
            .FirstOrDefault();

        if (candidato is null)
            return Result.Conflict<int>("No hay vehículos con capacidad disponible para este envío.");

        return Result.Success(candidato.IdConductor);
    }
}
