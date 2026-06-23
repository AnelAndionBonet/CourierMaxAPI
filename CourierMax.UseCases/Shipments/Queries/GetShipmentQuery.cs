using CourierMax.Data;
using CourierMax.Dtos.Shipments;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace CourierMax.UseCases;

/// <summary>Caso de uso: consultar un envío por su Id.</summary>
public class GetShipmentQuery
{
	private readonly IDbContextFactory<CourierMaxContext> _db;

	public GetShipmentQuery(IDbContextFactory<CourierMaxContext> db) => _db = db;

	public Task<Result<ShipmentResponse>> ExecuteAsync(Guid input, CancellationToken ct = default)
		=> Consultar(input, ct);

	private async Task<Result<ShipmentResponse>> Consultar(Guid id, CancellationToken ct)
	{
		await using var ctx = await _db.CreateDbContextAsync(ct);

		var envio = await ctx.Envios
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(x => new ShipmentResponse(x.Id, x.CodigoRastreo, x.Estado.Nomenclatura, x.Costo))
			.FirstOrDefaultAsync(ct);

		return envio is null
			? Result.NotFound<ShipmentResponse>("Envío no encontrado.")
			: envio.Success();
	}
}
