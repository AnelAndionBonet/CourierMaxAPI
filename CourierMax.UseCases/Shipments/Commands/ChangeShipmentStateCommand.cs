using CourierMax.Data;
using CourierMax.Data.Entities;
using CourierMax.Dtos.Shipments;
using Microsoft.EntityFrameworkCore;
using ROP;

namespace CourierMax.UseCases;

/// <summary>Caso de uso: cambiar el estado de un envío (RF-02, RN-03). Compuesto como un camino.</summary>
public class ChangeShipmentStateCommand
{
    private readonly IDbContextFactory<CourierMaxContext> _db;

    public ChangeShipmentStateCommand(IDbContextFactory<CourierMaxContext> db) => _db = db;

    public async Task<Result<ShipmentResponse>> ExecuteAsync(ChangeStateInput input, CancellationToken ct = default)
    {
        await using var ctx = await _db.CreateDbContextAsync(ct);

        return await Cargar(ctx, input, ct)
            .Bind(c => ResolverEstados(c))
            .Bind(c => ValidarTransicion(c))
            .Bind(c => ValidarCancelacion(c))
            .Bind(c => ResolverEstadoNuevoId(ctx, c, ct))
            .Bind(c => Aplicar(ctx, c, ct));
    }

    private sealed record Cambio
    {
        public required ChangeStateInput Input { get; init; }
        public required Guid Id { get; init; }
        public required string CodigoRastreo { get; init; }
        public required int IdEstadoActual { get; init; }
        public required string EstadoActualNom { get; init; }
        public required decimal Costo { get; init; }
        public EstadoEnvio EstadoActual { get; init; }
        public EstadoEnvio EstadoDestino { get; init; }
        public int IdEstadoNuevo { get; init; }
    }

    private async Task<Result<Cambio>> Cargar(CourierMaxContext ctx, ChangeStateInput input, CancellationToken ct)
    {
        var e = await ctx.Envios
            .AsNoTracking()
            .Where(x => x.Id == input.IdEnvio)
            .Select(x => new { x.Id, x.CodigoRastreo, x.IdEstado, EstadoNom = x.Estado.Nomenclatura, x.Costo })
            .FirstOrDefaultAsync(ct);

        return e is null
            ? Result.NotFound<Cambio>("Envío no encontrado.")
            : new Cambio
            {
                Input = input,
                Id = e.Id,
                CodigoRastreo = e.CodigoRastreo,
                IdEstadoActual = e.IdEstado,
                EstadoActualNom = e.EstadoNom,
                Costo = e.Costo,
            };
    }

    private Result<Cambio> ResolverEstados(Cambio c)
    {
        var actual = EstadoEnvioExtensions.DesdeNomenclatura(c.EstadoActualNom);
        if (actual is null) return Result.Failure<Cambio>("El estado actual del envío no es válido.");

        var destino = EstadoEnvioExtensions.DesdeNomenclatura(c.Input.EstadoDestino);
        if (destino is null) return Result.BadRequest<Cambio>($"Estado destino inválido: '{c.Input.EstadoDestino}'.");

        return c with { EstadoActual = actual.Value, EstadoDestino = destino.Value };
    }

    private Result<Cambio> ValidarTransicion(Cambio c)
        => ShipmentStateMachine.PuedeTransicionar(c.EstadoActual, c.EstadoDestino)
            ? c
            : Result.Conflict<Cambio>($"Transición inválida de {c.EstadoActual.Nomenclatura()} a {c.EstadoDestino.Nomenclatura()}.");

    private Result<Cambio> ValidarCancelacion(Cambio c)
    {
        if (c.EstadoDestino == EstadoEnvio.Cancelado &&
            (string.IsNullOrWhiteSpace(c.Input.Motivo) || c.Input.Motivo.Trim().Length < 5))
            return Result.BadRequest<Cambio>("La cancelación requiere un motivo de al menos 5 caracteres.");

        return c;
    }

    private async Task<Result<Cambio>> ResolverEstadoNuevoId(CourierMaxContext ctx, Cambio c, CancellationToken ct)
    {
        var id = await ctx.Estados.AsNoTracking()
            .Where(e => e.Nomenclatura == c.EstadoDestino.Nomenclatura())
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync(ct);

        return id is null
            ? Result.Failure<Cambio>($"No está configurado el estado {c.EstadoDestino.Nomenclatura()}.")
            : c with { IdEstadoNuevo = id.Value };
    }

    private async Task<Result<ShipmentResponse>> Aplicar(CourierMaxContext ctx, Cambio c, CancellationToken ct)
    {
        var ahora = DateTime.UtcNow;
        var envio = await ctx.Envios.FirstOrDefaultAsync(e => e.Id == c.Id, ct);
        if (envio is null) return Result.NotFound<ShipmentResponse>("Envío no encontrado.");

        envio.IdEstado = c.IdEstadoNuevo;
        envio.FechaModificacion = ahora;
        envio.ModificadoPor = c.Input.Usuario;
        if (c.EstadoDestino == EstadoEnvio.Entregado)
            envio.FechaEntrega = ahora;

        ctx.HistorialEstados.Add(new HistorialEstado
        {
            IdEnvio = c.Id,
            IdEstadoAnterior = c.IdEstadoActual,
            IdEstadoNuevo = c.IdEstadoNuevo,
            FechaCambio = ahora,
            Motivo = c.Input.Motivo,
            UsuarioCambio = c.Input.Usuario,
            FechaCreacion = ahora,
            CreadoPor = c.Input.Usuario,
        });
        await ctx.SaveChangesAsync(ct);

        return Result.Success(new ShipmentResponse(c.Id, c.CodigoRastreo, c.EstadoDestino.Nomenclatura(), c.Costo));
    }
}
