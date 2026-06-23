using CourierMax.Data;
using CourierMax.Data.Entities;
using CourierMax.Dtos.Shipments;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace CourierMax.UseCases;

/// <summary>Caso de uso: asignar automáticamente un conductor a un envío (RF-03, RN-01). Compuesto como un camino.</summary>
public class AssignShipmentCommand
{
	private readonly IDbContextFactory<CourierMaxContext> _db;

	public AssignShipmentCommand(IDbContextFactory<CourierMaxContext> db) => _db = db;

	public async Task<Result<ShipmentResponse>> ExecuteAsync(AssignShipmentInput input, CancellationToken ct = default)
	{
		await using var ctx = await _db.CreateDbContextAsync(ct);

		return await Cargar(ctx, input, ct)
			.Bind(ValidarTransicion)
			.Bind(a => SeleccionarConductor(ctx, a, ct))
			.Bind(a => ResolverEstadoAsignadoId(ctx, a, ct))
			.Bind(a => Aplicar(ctx, a, ct));
	}


	private async Task<Result<Asignacion>> Cargar(CourierMaxContext ctx, AssignShipmentInput input, CancellationToken ct)
	{
		var e = await ctx.Envios
			.AsNoTracking()
			.Where(x => x.Id == input.IdEnvio)
			.Select(x => new { x.Id, x.CodigoRastreo, x.IdEstado, EstadoNom = x.Estado.Nomenclatura, x.Peso, x.Largo, x.Ancho, x.Alto, x.Costo })
			.FirstOrDefaultAsync(ct);

		if (e is null) return Result.NotFound<Asignacion>("Envío no encontrado.");

		return new Asignacion
		{
			Input = input,
			Id = e.Id,
			CodigoRastreo = e.CodigoRastreo,
			IdEstadoActual = e.IdEstado,
			EstadoActualNom = e.EstadoNom,
			Peso = e.Peso,
			VolumenM3 = e.Largo * e.Ancho * e.Alto / 1_000_000m,
			Costo = e.Costo,
		};
	}

	private Result<Asignacion> ValidarTransicion(Asignacion a)
	{
		var actual = EstadoEnvioExtensions.DesdeNomenclatura(a.EstadoActualNom);
		if (actual is null) return Result.Failure<Asignacion>("El estado actual del envío no es válido.");

		return ShipmentStateMachine.PuedeTransicionar(actual.Value, EstadoEnvio.Asignado)
			? a
			: Result.Conflict<Asignacion>($"No se puede asignar un envío en estado {actual.Value.Nomenclatura()}.");
	}

	private async Task<Result<Asignacion>> SeleccionarConductor(CourierMaxContext ctx, Asignacion a, CancellationToken ct)
	{
		var cargas = await ObtenerCargas(ctx, ct);
		var seleccion = VehicleAssignmentService.Seleccionar(cargas, a.Peso, a.VolumenM3);

		return seleccion.Success
			? a with { IdConductor = seleccion.Value }
			: Result.Conflict<Asignacion>(seleccion.Errors);
	}

	private async Task<Result<Asignacion>> ResolverEstadoAsignadoId(CourierMaxContext ctx, Asignacion a, CancellationToken ct)
	{
		var id = await ctx.Estados
			.AsNoTracking()
			.Where(e => e.Nomenclatura == EstadoEnvio.Asignado.Nomenclatura())
			.Select(e => (int?)e.Id)
			.FirstOrDefaultAsync(ct);

		return id is null
			? Result.Failure<Asignacion>("No está configurado el estado ASIGNADO.")
			: a with { IdEstadoAsignado = id.Value };
	}

	private async Task<Result<ShipmentResponse>> Aplicar(CourierMaxContext ctx, Asignacion a, CancellationToken ct)
	{
		var ahora = DateTime.UtcNow;
		var envio = await ctx.Envios.FirstOrDefaultAsync(e => e.Id == a.Id, ct);
		if (envio is null) return Result.NotFound<ShipmentResponse>("Envío no encontrado.");

		envio.IdConductor = a.IdConductor;
		envio.FechaAsignacion = ahora;
		envio.IdEstado = a.IdEstadoAsignado;
		envio.FechaModificacion = ahora;
		envio.ModificadoPor = a.Input.Usuario;

		ctx.HistorialEstados.Add(new HistorialEstado
		{
			IdEnvio = a.Id,
			IdEstadoAnterior = a.IdEstadoActual,
			IdEstadoNuevo = a.IdEstadoAsignado,
			FechaCambio = ahora,
			Motivo = null,
			UsuarioCambio = a.Input.Usuario,
			FechaCreacion = ahora,
			CreadoPor = a.Input.Usuario,
		});
		await ctx.SaveChangesAsync(ct);

		return new ShipmentResponse(a.Id, a.CodigoRastreo, EstadoEnvio.Asignado.Nomenclatura(), a.Costo).Success();
	}

	/// <summary>Carga actual (peso/volumen) de cada conductor activo. (RN-01)</summary>
	private static async Task<IReadOnlyList<CargaConductorData>> ObtenerCargas(CourierMaxContext ctx, CancellationToken ct)
	{
		var conductores = await ctx.Conductores
			.AsNoTracking()
			.Where(c => c.Activo)
			.Select(c => new { c.Id, c.IdVehiculo, c.Vehiculo.CapacidadPesoKg, c.Vehiculo.CapacidadVolumenM3 })
			.ToListAsync(ct);

		var cargasActivas = await ctx.Envios
			.AsNoTracking()
			.Where(e => e.IdConductor != null
				&& (e.Estado.Nomenclatura == "ASIGNADO" || e.Estado.Nomenclatura == "EN_TRANSITO"))
			.GroupBy(e => e.IdConductor!.Value)
			.Select(g => new { Id = g.Key, Peso = g.Sum(x => x.Peso), VolCm3 = g.Sum(x => x.Largo * x.Ancho * x.Alto) })
			.ToListAsync(ct);

		var dict = cargasActivas.ToDictionary(c => c.Id);

		return conductores.Select(c =>
		{
			dict.TryGetValue(c.Id, out var carga);
			return new CargaConductorData(
				c.Id, c.IdVehiculo, Activo: true,
				c.CapacidadPesoKg, c.CapacidadVolumenM3,
				PesoActualKg: carga?.Peso ?? 0m,
				VolumenActualM3: (carga?.VolCm3 ?? 0m) / 1_000_000m);
		}).ToList();
	}
	private sealed record Asignacion
	{
		public required AssignShipmentInput Input { get; init; }
		public required Guid Id { get; init; }
		public required string CodigoRastreo { get; init; }
		public required int IdEstadoActual { get; init; }
		public required string EstadoActualNom { get; init; }
		public required decimal Peso { get; init; }
		public required decimal VolumenM3 { get; init; }
		public required decimal Costo { get; init; }
		public int IdConductor { get; init; }
		public int IdEstadoAsignado { get; init; }
	}
}
