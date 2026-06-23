using CourierMax.Data;
using CourierMax.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourierMax.API.Middleware
{
    /// <summary>
    /// Manejo centralizado de excepciones no controladas: loguea a consola,
    /// registra el error en BD (best-effort) y responde 500 con un traceId.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IDbContextFactory<CourierMaxContext> _dbFactory;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IDbContextFactory<CourierMaxContext> dbFactory)
        {
            _next = next;
            _logger = logger;
            _dbFactory = dbFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var traceId = context.TraceIdentifier;

                // 1. Log a consola (siempre, es el respaldo si la BD también falla)
                _logger.LogError(ex, "Error no controlado en {Path} (TraceId: {TraceId})", context.Request.Path, traceId);

                // 2. Persistir en BD — best-effort: nunca debe romper la respuesta
                await RegistrarEnBdAsync(context, ex, traceId);

                // 3. Responder 500 con cuerpo genérico + traceId para correlacionar
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Ocurrió un error inesperado.", traceId });
            }
        }

        private async Task RegistrarEnBdAsync(HttpContext context, Exception ex, string traceId)
        {
            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();
                db.RegistroErrores.Add(new RegistroError
                {
                    FechaUtc = DateTime.UtcNow,
                    Ruta = Truncar(context.Request.Path, 500),
                    Metodo = context.Request.Method,
                    Mensaje = Truncar(ex.Message, 2000),
                    Detalle = ex.ToString(),
                    TraceId = traceId,
                });
                await db.SaveChangesAsync();
            }
            catch (Exception logEx)
            {
                // Si falla guardar el error (p. ej. la BD está caída), solo lo registramos en consola.
                _logger.LogError(logEx, "No se pudo persistir el error en la base de datos (best-effort).");
            }
        }

        private static string Truncar(string? valor, int max)
        {
            valor ??= string.Empty;
            return valor.Length <= max ? valor : valor[..max];
        }
    }
}
