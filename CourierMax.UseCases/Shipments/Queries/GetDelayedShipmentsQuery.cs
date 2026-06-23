using CourierMax.Data;
using CourierMax.Dtos.Shipments;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace CourierMax.UseCases;

/// <summary>Caso de uso: listar envíos con SLA vencido en un rango de fechas (RF-05, RN-02). Compuesto como un camino.</summary>
public class GetDelayedShipmentsQuery
{
	private readonly IDbContextFactory<CourierMaxContext> _db;

	public GetDelayedShipmentsQuery(IDbContextFactory<CourierMaxContext> db) => _db = db;

	public async Task<Result<IReadOnlyList<DelayedShipmentDto>>> ExecuteAsync(DateRangeInput input, CancellationToken ct = default)
	{
		await using var ctx = await _db.CreateDbContextAsync(ct);

		return await ValidarRango(input)
			.Bind(r => ConsultarActivos(ctx, r, ct))
			.Map(activos => FiltrarAtrasados(activos));
	}

	private sealed record EnvioActivo(Guid Id, string CodigoRastreo, DateTime FechaCreacion, string ServicioNom, string EstadoNom);

	private static Result<DateRangeInput> ValidarRango(DateRangeInput input)
		=> input.Hasta < input.Desde
			? Result.BadRequest<DateRangeInput>("El rango de fechas es inválido.")
			: input;

	private async Task<Result<IReadOnlyList<EnvioActivo>>> ConsultarActivos(CourierMaxContext ctx, DateRangeInput r, CancellationToken ct)
	{
		var activos = await ctx.Envios
			.AsNoTracking()
			.Where(e => e.FechaCreacion >= r.Desde
				&& e.FechaCreacion <= r.Hasta
				&& e.Estado.Nomenclatura != "ENTREGADO"
				&& e.Estado.Nomenclatura != "CANCELADO")
			.Select(e => new EnvioActivo(e.Id, e.CodigoRastreo, e.FechaCreacion, e.TipoServicio.Nomenclatura, e.Estado.Nomenclatura))
			.ToListAsync(ct);

		return activos;
	}

	private static IReadOnlyList<DelayedShipmentDto> FiltrarAtrasados(IReadOnlyList<EnvioActivo> activos)
	{
		var ahora = DateTime.UtcNow;
		var atrasados = new List<DelayedShipmentDto>();

		foreach (var e in activos)
		{
			var sla = SlaPolicy.DiasHabiles(e.ServicioNom);
			var transcurridos = BusinessDayCalculator.BusinessDaysBetween(e.FechaCreacion, ahora);
			if (transcurridos > sla)
				atrasados.Add(new DelayedShipmentDto(e.Id, e.CodigoRastreo, e.ServicioNom, e.EstadoNom, e.FechaCreacion, transcurridos, sla));
		}

		return atrasados;
	}
}
