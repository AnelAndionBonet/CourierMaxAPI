# CourierMax — Guía del proyecto

API REST de gestión de envíos (courier) en **.NET 9 / ASP.NET Core**. Idioma del proyecto: **español** (nombres de dominio, mensajes, comentarios).

## Arquitectura

**Core-Driven (estilo NetMentor)** — híbrido pragmático de Clean + Layered. El Core de casos de uso manda; NO se usa patrón repositorio (los casos de uso usan EF Core directamente).

```
  CourierMax.API  ───►  CourierMax.UseCases  ───►  CourierMax.Data
   (Entry Point)            (Core)                  (Infraestructura/EF)
                               │
                               ▼
                        CourierMax.Dtos (Contratos)
```

| Proyecto | Rol | Referencia |
|----------|-----|-----------|
| `CourierMaxAPI` (`CourierMax.API`) | Controllers delgados, middleware, wiring DI | → UseCases, Data |
| `CourierMax.UseCases` | **Core**: Commands/Queries, dominio, validadores | → Data, Dtos |
| `CourierMax.Data` | EF Core: entidades, `CourierMaxContext`, configuraciones, migraciones | (hoja) |
| `CourierMax.Dtos` | DTOs de entrada/salida | (hoja) |
| `tests/CourierMax.UseCases.Tests` | Pruebas unitarias (xUnit) | → UseCases, Data |

## Paquetes clave

- `Netmentor.ROP 1.12.0` — `Result<T>` (lógica, en UseCases)
- `Netmentor.ROP.ApiExtensions 1.12.0` — `ToActionResult()` (API)
- `FluentValidation 12.x` — validación de inputs (UseCases)
- `Microsoft.EntityFrameworkCore.SqlServer 9.0.0` (Data)
- `Swashbuckle.AspNetCore` — Swagger UI en `/swagger` (API)
- Pruebas: `xUnit`, `FluentAssertions 6.12.1`, `NSubstitute`, `Microsoft.EntityFrameworkCore.InMemory`

## Flujo de una petición

```
Controller → Module record (XUseCases) → Command/Query.ExecuteAsync → IDbContextFactory<CourierMaxContext> (EF) → Result<T> → ToActionResult()
```

## Patrones obligatorios

### Namespace plano
**Todas** las clases de `CourierMax.UseCases` usan `namespace CourierMax.UseCases;` sin importar la subcarpeta (las carpetas `Shipments/`, `Drivers/`, `Commands/`, `Queries/`, `Domain/` son solo organización física). Igual criterio que la regla de DTOs.

### Caso de uso (Command/Query) — railway con responsabilidad única
- Una clase por caso de uso. `*Command` = escritura, `*Query` = lectura.
- Método público único `ExecuteAsync(TInput input, CancellationToken ct = default)` que devuelve `Task<Result<TOut>>`. **No hay clase base.**
- El cuerpo se compone como un **camino**: pasos privados de una sola responsabilidad encadenados con `.Bind`/`.Map`. Si un paso falla, el resto se salta.
- Acceso a datos con `IDbContextFactory<CourierMaxContext>` directo: `await using var ctx = await _db.CreateDbContextAsync(ct);`
- Lecturas: `AsNoTracking()` + proyección directa a DTO con `.Select()`. Nunca devolver entidades.

```csharp
public class GetShipmentQuery
{
    private readonly IDbContextFactory<CourierMaxContext> _db;
    public GetShipmentQuery(IDbContextFactory<CourierMaxContext> db) => _db = db;

    public async Task<Result<ShipmentResponse>> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        await using var ctx = await _db.CreateDbContextAsync(ct);
        var envio = await ctx.Envios.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ShipmentResponse(x.Id, x.CodigoRastreo, x.Estado.Nomenclatura, x.Costo))
            .FirstOrDefaultAsync(ct);
        return envio is null ? Result.NotFound<ShipmentResponse>("Envío no encontrado.") : envio.Success();
    }
}
```

```csharp
// Camino de un Command: cada Bind es un paso con una responsabilidad
public async Task<Result<ShipmentResponse>> ExecuteAsync(CreateShipmentRequest req, CancellationToken ct = default)
{
    await using var ctx = await _db.CreateDbContextAsync(ct);
    return await Validar(req)
        .Bind(r => ResolverCiudades(ctx, r, ct))
        .Bind(b => ResolverTarifa(ctx, b, ct))
        .Bind(b => CalcularCosto(b))
        .Bind(b => Persistir(ctx, b, ct));
}
```

### Record contenedor por módulo
```csharp
public record class ShipmentsUseCases(
    CreateShipmentCommand CreateShipmentCommand,
    ChangeShipmentStateCommand ChangeShipmentStateCommand,
    GetShipmentQuery GetShipmentQuery /* ... */);
```

### Controlador (delgado)
Hereda `BaseController` (placeholder, sin auth), constructor primario con el record del módulo, y `.ToActionResult()` (de `ROP.APIExtensions`).
```csharp
[ApiController]
[Route("api/shipments")]
public class ShipmentsController(ShipmentsUseCases uc) : BaseController
{
    [HttpPost]
    public Task<IActionResult> Crear([FromBody] CreateShipmentRequest req)
        => uc.CreateShipmentCommand.ExecuteAsync(req).ToActionResult();
}
```

### Validador (FluentValidation)
```csharp
public class CreateShipmentValidator : AbstractValidator<CreateShipmentRequest>
{
    public CreateShipmentValidator()
    {
        RuleFor(x => x.Peso).InclusiveBetween(0.1m, 100m).WithMessage("El peso debe estar entre 0.1 y 100 kg.");
        // ...
    }
}
```

### Retornos ROP
| Método | Cuándo | HTTP |
|--------|--------|------|
| `Result.Success(valor)` / `valor.Success()` | OK | 200 |
| `Result.Success(valor, HttpStatusCode.Created)` | Creación | 201 |
| `Result.BadRequest<T>("msg")` | Validación / regla de negocio | 400 |
| `Result.NotFound<T>("msg")` | No existe el registro | 404 |
| `Result.Conflict<T>("msg")` | Conflicto (ej. transición o capacidad) | 409 |
| `Result.Failure<T>("msg")` | Error técnico/configuración | 400 |

**No hay `BusinessException`.** Errores como valores (Result). Las excepciones inesperadas suben al middleware → 500.

### Registro en DI
```csharp
// UseCasesDependencyInjection.AddUseCases
services.AddScoped<CreateShipmentValidator>();
services.AddScoped<IValidator<CreateShipmentRequest>, CreateShipmentValidator>();
services.AddScoped<CreateShipmentCommand>();
services.AddScoped<ShipmentsUseCases>();
// DataDependencyInjection.AddData
services.AddDbContextFactory<CourierMaxContext>(o => o.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));
```
Al agregar un caso de uso: **(1)** crear la clase · **(2)** agregar al record del módulo · **(3)** registrar en `AddUseCases`.

## Modelo de datos

- **Esquemas:** `gral` (catálogos: `Estados`, `Ciudades`, `TipoDetalles`, `DistanciasTarifas`, `RegistroErrores`) y `core` (negocio: `Clientes`, `Envios`, `Vehiculos`, `Conductores`, `HistorialEstados`).
- Entidades en `CourierMax.Data/Entities`, heredan `EntidadAuditable<TKey>` (campos de auditoría: `FechaCreacion`, `CreadoPor`, `FechaModificacion`, `ModificadoPor`, `FechaAnulacion`, `AnuladoPor`, `ObservacionEstado`). Excepción: `RegistroError` (tabla de sistema) no audita.
- Configuraciones Fluent en `CourierMax.Data/Configurations`: `varchar` con colación `SQL_Latin1_General_CP1_CI_AS` (helper `ColumnaTexto(n)`), FKs `OnDelete(Restrict)`.
- `Envio.Id` es `Guid` con `NEWSEQUENTIALID()`; las PK de catálogos son `int IDENTITY`.
- `TipoDetalles` es un catálogo **plano** (unidades, tipos de servicio, tipos de paquete, tipos de identificación) diferenciado por `Nomenclatura`.
- El origen/destino y las direcciones de un envío **se derivan del Cliente** (remitente/destinatario); el envío no los duplica.
- **Esquema por migraciones EF Core** (code-first). **Datos semilla por scripts SQL manuales** en `CourierMax.Data/Scripts` (no `HasData`).

### Dominio (en `CourierMax.UseCases`)
`EstadoEnvio` (+`Nomenclatura()`/`DesdeNomenclatura()`), `ShipmentStateMachine`, `TariffCalculator`, `BusinessDayCalculator` (festivos CO 2026), `SlaPolicy`, `VehicleAssignmentService`.

Estados: `CREADO → ASIGNADO → EN_TRANSITO → ENTREGADO`; `CANCELADO` desde cualquiera excepto `ENTREGADO`.

## Manejo de errores y logging

`ExceptionHandlingMiddleware` (API): captura excepciones no controladas → log a consola + registro **best-effort** en `gral.RegistroErrores` (su propio try/catch, vía `IDbContextFactory`, nunca propaga) → responde **500** con `{ error, traceId }`. Los fallos de negocio NO pasan por aquí (son `Result`).

## Pruebas

`tests/CourierMax.UseCases.Tests` (xUnit). Dominio: pruebas puras. Casos de uso: `TestDbContextFactory` (EF InMemory aislado) + `SeedHelper`. Correr: `dotnet test`.
> EF InMemory ignora `NEWSEQUENTIALID()`/tipos `varchar`; en pruebas de creación el `Id` queda vacío (no afirmar sobre él).

## Reglas de trabajo (para el agente)

- **Git:** NO ejecutar `git add` ni `git commit`. El usuario versiona manualmente.
- **Paquetes/proyectos:** NO instalar paquetes NuGet, crear proyectos ni editar `.csproj`/`.sln` sin pedirlo explícitamente. El `.sln` lo gestiona el usuario desde Visual Studio (no editarlo por CLI con VS abierto: VS lo sobreescribe).
- **Pruebas:** no escribir pruebas unitarias salvo que se pidan; verificar por **compilación** (`dotnet build`).
- **Datos semilla:** generar scripts SQL `INSERT` (idempotentes) para que el usuario los ejecute; no usar `HasData`.
- **Build con VS abierto:** si los DLL están bloqueados, compilar a temporal:
  ```bash
  OUT="$(cygpath -u "$TEMP")/build-temp"; dotnet build CourierMaxAPI/CourierMax.API.csproj -o "$OUT" --no-incremental -v q -nologo 2>&1 | tail -5
  ```
- **EF migraciones:** `dotnet ef ... --project CourierMax.Data/CourierMax.Data.csproj --startup-project CourierMaxAPI/CourierMax.API.csproj` (la cadena vive en `appsettings.Development.json`, entorno `Development`).
</content>
