using CourierMax.Data;
using CourierMax.Dtos.Driver;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace CourierMax.UseCases;

/// <summary>Caso de uso: reporte de métricas de eficiencia por conductor (RF-06). Compuesto como un camino.</summary>
public class GetDriverMetricsQuery
{
	private readonly IDbContextFactory<CourierMaxContext> _db;

	public GetDriverMetricsQuery(IDbContextFactory<CourierMaxContext> db) => _db = db;

	public async Task<Result<DriverMetricsDto>> ExecuteAsync(int input, CancellationToken ct = default)
	{
		await using var ctx = await _db.CreateDbContextAsync(ct);

		return await ObtenerConductor(ctx, input, ct)
			.Bind(nombre => CargarEnvios(ctx, input, nombre, ct))
			.Map(datos => CalcularMetricas(input, datos));
	}

	private sealed record EnvioMetrica(string EstadoNom, string ServicioNom, decimal Peso, DateTime FechaCreacion, DateTime? FechaAsignacion, DateTime? FechaEntrega);

	private sealed record Datos(string Nombre, IReadOnlyList<EnvioMetrica> Envios);

	private async Task<Result<string>> ObtenerConductor(CourierMaxContext ctx, int id, CancellationToken ct)
	{
		var nombre = await ctx.Conductores
			.AsNoTracking()
			.Where(c => c.Id == id)
			.Select(c => (string?)c.Nombre)
			.FirstOrDefaultAsync(ct);

		return nombre is null ? Result.NotFound<string>("Conductor no encontrado.") : nombre;
	}

	private async Task<Result<Datos>> CargarEnvios(CourierMaxContext ctx, int id, string nombre, CancellationToken ct)
	{
		var envios = await ctx.Envios
			.AsNoTracking()
			.Where(e => e.IdConductor == id)
			.Select(e => new EnvioMetrica(
				e.Estado.Nomenclatura,
				e.TipoServicio.Nomenclatura,
				e.Peso,
				e.FechaCreacion,
				e.FechaAsignacion,
				e.FechaEntrega))
			.ToListAsync(ct);

		return new Datos(nombre, envios);
	}

	private static DriverMetricsDto CalcularMetricas(int idConductor, Datos datos)
	{
		var envios = datos.Envios;
		var entregadoNom = EstadoEnvio.Entregado.Nomenclatura();
		var canceladoNom = EstadoEnvio.Cancelado.Nomenclatura();
		var enTransitoNom = EstadoEnvio.EnTransito.Nomenclatura();

		var entregados = envios.Where(e => e.EstadoNom == entregadoNom).ToList();

		int total = envios.Count;
		int nEntregados = entregados.Count;
		int nCancelados = envios.Count(e => e.EstadoNom == canceladoNom);
		int nEnTransito = envios.Count(e => e.EstadoNom == enTransitoNom);

		var entregadosConFechas = entregados
			.Where(e => e.FechaAsignacion.HasValue && e.FechaEntrega.HasValue)
			.ToList();

		double tiempoPromedio = entregadosConFechas.Count > 0
			? entregadosConFechas.Average(e => BusinessDayCalculator.BusinessDaysBetween(e.FechaAsignacion!.Value, e.FechaEntrega!.Value))
			: 0d;

		int dentroSla = entregados.Count(e =>
			e.FechaEntrega.HasValue &&
			BusinessDayCalculator.BusinessDaysBetween(e.FechaCreacion, e.FechaEntrega!.Value) <= SlaPolicy.DiasHabiles(e.ServicioNom));

		double porcentajeSla = nEntregados > 0 ? (double)dentroSla / nEntregados * 100d : 0d;

		decimal pesoTotal = envios.Sum(e => e.Peso);

		return new DriverMetricsDto(
			idConductor, datos.Nombre, total, nEntregados, nCancelados, nEnTransito,
			Math.Round(tiempoPromedio, 2), Math.Round(porcentajeSla, 2), pesoTotal);
	}
}
