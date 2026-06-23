using System.Net;
using CourierMax.Data;
using CourierMax.Data.Entities;
using CourierMax.Dtos.Shipments;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace CourierMax.UseCases;

/// <summary>Caso de uso: crear un envío (RF-01). El flujo se compone como un camino (railway).</summary>
public class CreateShipmentCommand
{
	private readonly IDbContextFactory<CourierMaxContext> _db;
	private readonly CreateShipmentValidator _validator;

	public CreateShipmentCommand(IDbContextFactory<CourierMaxContext> db, CreateShipmentValidator validator)
	{
		_db = db;
		_validator = validator;
	}

	public async Task<Result<ShipmentResponse>> ExecuteAsync(CreateShipmentRequest req, CancellationToken ct = default)
	{
		await using var ctx = await _db.CreateDbContextAsync(ct);

		return await Validar(req)
			.Bind(r => ResolverCiudades(ctx, r, ct))
			.Bind(b => ResolverTarifa(ctx, b, ct))
			.Bind(b => ResolverCatalogos(ctx, b, ct))
			.Bind(b => CalcularCosto(b))
			.Bind(b => GenerarCodigoRastreo(ctx, b, ct))
			.Bind(b => Persistir(ctx, b, ct));
	}

	/// <summary>Datos que se van acumulando a lo largo del camino.</summary>
	private sealed record Borrador
	{
		public required CreateShipmentRequest Req { get; init; }
		public int CiudadOrigen { get; init; }
		public int CiudadDestino { get; init; }
		public decimal TarifaDistancia { get; init; }
		public string ServicioNom { get; init; } = "";
		public string PaqueteNom { get; init; } = "";
		public int IdEstadoCreado { get; init; }
		public decimal Costo { get; init; }
		public string CodigoRastreo { get; init; } = "";
	}

	// ── Pasos del camino (una responsabilidad cada uno) ───────────────────────

	private async Task<Result<CreateShipmentRequest>> Validar(CreateShipmentRequest req)
	{
		var v = await _validator.ValidateAsync(req);
		return v.IsValid ? req : Result.BadRequest<CreateShipmentRequest>(v.Errors[0].ErrorMessage);
	}

	private async Task<Result<Borrador>> ResolverCiudades(CourierMaxContext ctx, CreateShipmentRequest req, CancellationToken ct)
	{
		var origen = await CiudadDeCliente(ctx, req.IdRemitente, ct);
		if (origen is null) return Result.BadRequest<Borrador>("El remitente no existe.");

		var destino = await CiudadDeCliente(ctx, req.IdDestinatario, ct);
		if (destino is null) return Result.BadRequest<Borrador>("El destinatario no existe.");

		if (origen.Value == destino.Value)
			return Result.BadRequest<Borrador>("El remitente y el destinatario están en la misma ciudad; no aplica tarifa de distancia.");

		return new Borrador { Req = req, CiudadOrigen = origen.Value, CiudadDestino = destino.Value };
	}

	private async Task<Result<Borrador>> ResolverTarifa(CourierMaxContext ctx, Borrador b, CancellationToken ct)
	{
		var tarifa = await ctx.DistanciasTarifas
			.AsNoTracking()
			.Where(d => d.IdCiudadOrigen == b.CiudadOrigen && d.IdCiudadDestino == b.CiudadDestino
					 || d.IdCiudadOrigen == b.CiudadDestino && d.IdCiudadDestino == b.CiudadOrigen)
			.Select(d => (decimal?)d.TarifaDistancia)
			.FirstOrDefaultAsync(ct);

		return tarifa is null
			? Result.BadRequest<Borrador>("No hay tarifa de distancia definida entre las ciudades del remitente y el destinatario.")
			: b with { TarifaDistancia = tarifa.Value };
	}

	private async Task<Result<Borrador>> ResolverCatalogos(CourierMaxContext ctx, Borrador b, CancellationToken ct)
	{
		var servicioNom = await NomenclaturaTipoDetalle(ctx, b.Req.IdTipoServicio, ct);
		if (servicioNom is null) return Result.BadRequest<Borrador>("El tipo de servicio no existe.");

		var paqueteNom = await NomenclaturaTipoDetalle(ctx, b.Req.IdTipoPaquete, ct);
		if (paqueteNom is null) return Result.BadRequest<Borrador>("El tipo de paquete no existe.");

		var idCreado = await EstadoIdPorNomenclatura(ctx, EstadoEnvio.Creado.Nomenclatura(), ct);
		if (idCreado is null) return Result.Failure<Borrador>("No está configurado el estado CREADO.");

		return b with { ServicioNom = servicioNom, PaqueteNom = paqueteNom, IdEstadoCreado = idCreado.Value };
	}

	private Result<Borrador> CalcularCosto(Borrador b)
	{
		try
		{
			var costo = TariffCalculator.Calcular(new TarifaInput(b.ServicioNom, b.Req.Peso, b.TarifaDistancia, b.PaqueteNom));
			return b with { Costo = costo };
		}
		catch (ArgumentException ex)
		{
			return Result.BadRequest<Borrador>(ex.Message);
		}
	}

	private async Task<Result<Borrador>> GenerarCodigoRastreo(CourierMaxContext ctx, Borrador b, CancellationToken ct)
	{
		for (int i = 0; i < 5; i++)
		{
			var candidato = "CM-" + Random.Shared.Next(0, 100_000_000).ToString("D8");
			if (!await ctx.Envios.AsNoTracking().AnyAsync(e => e.CodigoRastreo == candidato, ct))
				return b with { CodigoRastreo = candidato };
		}
		return Result.Failure<Borrador>("No fue posible generar un código de rastreo único. Intente de nuevo.");
	}

	private async Task<Result<ShipmentResponse>> Persistir(CourierMaxContext ctx, Borrador b, CancellationToken ct)
	{
		var ahora = DateTime.UtcNow;
		var req = b.Req;

		var envio = new Envio
		{
			IdRemitente = req.IdRemitente,
			IdDestinatario = req.IdDestinatario,
			CodigoRastreo = b.CodigoRastreo,
			Peso = req.Peso,
			IdUnidadPeso = req.IdUnidadPeso,
			Largo = req.Largo,
			Ancho = req.Ancho,
			Alto = req.Alto,
			IdUnidadVolumen = req.IdUnidadVolumen,
			IdTipoPaquete = req.IdTipoPaquete,
			Costo = b.Costo,
			IdTipoServicio = req.IdTipoServicio,
			IdEstado = b.IdEstadoCreado,
			FechaCreacion = ahora,
			CreadoPor = req.Usuario,
		};

		ctx.Envios.Add(envio);
		await ctx.SaveChangesAsync(ct); // genera el Id (NEWSEQUENTIALID)

		ctx.HistorialEstados.Add(new HistorialEstado
		{
			IdEnvio = envio.Id,
			IdEstadoAnterior = null,
			IdEstadoNuevo = b.IdEstadoCreado,
			FechaCambio = ahora,
			Motivo = null,
			UsuarioCambio = req.Usuario,
			FechaCreacion = ahora,
			CreadoPor = req.Usuario,
		});
		await ctx.SaveChangesAsync(ct);

		var respuesta = new ShipmentResponse(envio.Id, envio.CodigoRastreo, EstadoEnvio.Creado.Nomenclatura(), b.Costo);
		return respuesta.Success(HttpStatusCode.Created);
	}

	// ── Consultas auxiliares de catálogo ──────────────────────────────────────

	private static Task<int?> CiudadDeCliente(CourierMaxContext ctx, int idCliente, CancellationToken ct)
		=> ctx.Clientes.AsNoTracking().Where(c => c.Id == idCliente).Select(c => (int?)c.IdCiudad).FirstOrDefaultAsync(ct);

	private static Task<string?> NomenclaturaTipoDetalle(CourierMaxContext ctx, int id, CancellationToken ct)
		=> ctx.TipoDetalles.AsNoTracking().Where(t => t.Id == id).Select(t => (string?)t.Nomenclatura).FirstOrDefaultAsync(ct);

	private static Task<int?> EstadoIdPorNomenclatura(CourierMaxContext ctx, string nom, CancellationToken ct)
		=> ctx.Estados.AsNoTracking().Where(e => e.Nomenclatura == nom).Select(e => (int?)e.Id).FirstOrDefaultAsync(ct);
}
