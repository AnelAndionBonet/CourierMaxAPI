# CourierMax API

API REST para la gestión del ciclo completo de envíos de una empresa de mensajería (courier): creación, seguimiento de estados, asignación a vehículos/conductores, cálculo de tarifas, alertas de SLA y métricas de eficiencia.

> Prueba técnica — Desarrollador .NET. .NET 9 (ASP.NET Core Web API).

## Arquitectura

Se adoptó la **arquitectura Core-Driven** (estilo NetMentor): el **Core de casos de uso** manda y las dependencias apuntan hacia él. Es un híbrido pragmático de Clean + Layered, evitando la ceremonia de repositorios cuando EF Core ya ofrece una abstracción de persistencia adecuada.

```
  CourierMax.API  ───►  CourierMax.UseCases  ◄───  CourierMax.Data
   (Entry Point)            (Core)                  (Infraestructura)
        │                      │                          │
        └────► (composición)   ▼                          │
                       CourierMax.Dtos  ◄─────────────────┘
```

| Proyecto | Rol |
|----------|-----|
| `CourierMaxAPI` (`CourierMax.API`) | Entry point: controllers delgados, middleware de errores, wiring de DI. |
| `CourierMax.UseCases` | **Core**: casos de uso (Commands/Queries), dominio (máquina de estados, cálculo de tarifa, días hábiles, SLA, asignación), validadores. |
| `CourierMax.Data` | Infraestructura: EF Core, entidades, `CourierMaxContext`, configuraciones y migraciones. |
| `CourierMax.Dtos` | Contratos de entrada/salida (DTOs). |
| `tests/CourierMax.UseCases.Tests` | Pruebas unitarias (xUnit). |

### Justificación y decisiones

- **Core-Driven sobre Clean estricta:** los casos de uso usan `IDbContextFactory<CourierMaxContext>` directamente (con `await using`), sin una capa de repositorios. Se gana simplicidad sin perder testabilidad (EF InMemory en pruebas). Las interfaces se reservan para límites externos reales.
- **Railway Oriented Programming con `Netmentor.ROP`:** los casos de uso devuelven `Result<T>`; los errores son **valores**, no excepciones. El flujo de cada caso de uso se compone como un *camino* de pasos de responsabilidad única encadenados con `.Bind`/`.Map`. El `HttpStatusCode` viaja con el `Result` y se traduce a HTTP con `ToActionResult()` (`Netmentor.ROP.ApiExtensions`).
- **CQRS ligero:** un caso de uso = una clase (`*Command` para escritura, `*Query` para lectura), con un único método público `ExecuteAsync`. Se agrupan por módulo en un record (`ShipmentsUseCases`, `DriversUseCases`).
- **DTOs siempre, nunca entidades:** los casos de uso devuelven DTOs (proyección directa con `.Select()` + `AsNoTracking()` en lecturas).
- **Origen/destino y direcciones derivados del Cliente:** un envío no duplica ciudad ni dirección; el origen es la ciudad/dirección del remitente y el destino las del destinatario.
- **Manejo centralizado de errores + logging:** middleware que captura excepciones no controladas → 500 con `traceId`, log a consola y registro *best-effort* en la tabla `gral.RegistroErrores`.

## Tecnologías

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 (SQL Server / Azure SQL)
- Netmentor.ROP 1.12.0 + Netmentor.ROP.ApiExtensions 1.12.0
- FluentValidation 12.x
- OpenAPI (`Microsoft.AspNetCore.OpenApi`)
- xUnit + FluentAssertions + NSubstitute + EF Core InMemory (pruebas)

## Cómo ejecutar el proyecto localmente

### Requisitos
- SDK de .NET 9
- SQL Server (local, Express, LocalDB o Azure SQL)
- Herramienta EF Core: `dotnet tool restore` (el repo incluye un manifest con `dotnet-ef`)

### 1. Configurar la cadena de conexión

La cadena de conexión se lee con `configuration.GetConnectionString("DefaultConnection")` desde la jerarquía de configuración de .NET.

**En Azure (App Service).** La cadena se define como **variable de entorno del App Service**, en la sección **Configuración → Cadenas de conexión**:

| Campo | Valor |
|-------|-------|
| Nombre | `DefaultConnection` |
| Valor | la cadena de conexión a Azure SQL |
| Tipo | `SQLAzure` |

El App Service la expone como variable de entorno (`SQLAZURECONNSTR_DefaultConnection`) que .NET mapea automáticamente a la sección `ConnectionStrings`; al tener mayor prioridad que los `appsettings`, es la que se usa en producción. Con esto, ni el repositorio ni los archivos `appsettings*.json` contienen credenciales.

> **Nota sobre Azure Key Vault.** La intención inicial era guardar la cadena en **Azure Key Vault** y referenciarla desde el App Service. Por falta de permisos para crear los secretos en el Key Vault, se optó por usar directamente la **cadena de conexión del App Service** (descrita arriba), que cumple el mismo objetivo de no versionar credenciales. **Queda como mejora futura** migrar la cadena a Key Vault y referenciarla con `@Microsoft.KeyVault(SecretUri=...)` usando una identidad administrada.

### 2. Crear el esquema (migraciones EF Core)

```bash
dotnet ef database update \
  --project CourierMax.Data/CourierMax.Data.csproj \
  --startup-project CourierMaxAPI/CourierMax.API.csproj
```

Esto crea los esquemas `gral` y `core` y todas las tablas.

### 3. Sembrar los datos de referencia
Ejecuta los scripts de `CourierMax.Data/Scripts/` en este orden (SSMS / Azure Data Studio):

1. `Seed_Estados.sql`, `Seed_Ciudades.sql`, `Seed_TipoDetalles.sql`
2. `Seed_Correcciones.sql`  *(agrega ENTREGADO/CANCELADO y tipos PAQ + unidades KG/M3)*
3. `Seed_DistanciasTarifas.sql`
4. `Seed_Vehiculos_Conductores.sql`
5. `Seed_Clientes.sql`

> Cada script trae un `SELECT` de verificación al final con los `Id` resultantes (los necesitarás para las peticiones).

### 4. Ejecutar el API

```bash
dotnet run --project CourierMaxAPI/CourierMax.API.csproj
```

- API: `https://localhost:7270` / `http://localhost:5096`
- Especificación OpenAPI: `http://localhost:5096/openapi/v1.json`

### 5. Ejecutar las pruebas

```bash
dotnet test
```

## Modelo de datos (tablas y relaciones)

La base se organiza en dos esquemas:

- **`gral`** — catálogos y datos de referencia (cambian poco): `Ciudades`, `Estados`, `TipoDetalles`, `DistanciasTarifas`, `RegistroErrores`.
- **`core`** — datos del negocio (el día a día): `Clientes`, `Vehiculos`, `Conductores`, `Envios`, `HistorialEstados`.

### Qué guarda cada tabla

| Esquema | Tabla | Qué contiene |
|---------|-------|--------------|
| gral | `Ciudades` | Ciudades válidas del sistema (Bogotá, Medellín, …). |
| gral | `Estados` | Catálogo de estados del envío (`CREADO`, `ASIGNADO`, …). |
| gral | `TipoDetalles` | Catálogo **plano**: unidades de peso/volumen, tipos de servicio, tipos de paquete y tipos de identificación (se distinguen por `Nomenclatura`). |
| gral | `DistanciasTarifas` | Distancia y tarifa entre un par de ciudades. |
| gral | `RegistroErrores` | Log de excepciones no controladas (tabla de sistema, sin relaciones de negocio). |
| core | `Clientes` | Remitentes y destinatarios (nombre, teléfono, dirección, ciudad). |
| core | `Vehiculos` | Vehículos con su capacidad de peso (kg) y volumen (m³). |
| core | `Conductores` | Conductores, su estado activo y el vehículo que manejan. |
| core | `Envios` | El envío: paquete, costo, estado actual, conductor asignado y fechas clave. |
| core | `HistorialEstados` | Una fila por cada cambio de estado de un envío (auditoría). |

### Cómo se relacionan (en lenguaje simple)

- Un **Cliente** está en una **Ciudad** y tiene un **tipo de identificación** (de `TipoDetalles`).
- Un **Conductor** maneja un **Vehículo** (en los datos de referencia es 1:1: cada conductor con su vehículo).
- Un **Envío** une casi todo el modelo:
  - tiene un **remitente** y un **destinatario** (ambos son **Clientes**); el origen y el destino se **derivan de la ciudad de cada uno** (el envío no repite direcciones).
  - apunta a su **Estado** actual.
  - referencia cuatro registros de **TipoDetalles**: unidad de peso, unidad de volumen, tipo de paquete y tipo de servicio.
  - puede tener (opcionalmente) un **Conductor** asignado.
- Cada vez que un envío cambia de estado se crea una fila en **HistorialEstados**, que recuerda el **estado anterior** y el **nuevo** (ambos de `Estados`). Un envío tiene muchas filas de historial (1 a N).
- Una **DistanciaTarifa** conecta dos **Ciudades** (origen y destino) y define cuánto se cobra por esa distancia.

### Diagrama de relaciones

```
gral.Ciudades ─┐
               ├──< core.Clientes >──┐ (remitente / destinatario)
               │                     │
gral.TipoDetalles ──(tipo ident.)────┘
               │
               └──< gral.DistanciasTarifas >── gral.Ciudades

gral.Vehiculos ──1:1── core.Conductores ──(opcional)──┐
                                                       │
core.Clientes ───(remitente, destinatario)───┐        │
gral.Estados ─────(estado actual)─────────────┤        │
gral.TipoDetalles ─(peso, volumen, paquete,   ├──> core.Envios <──┘
                    servicio)─────────────────┘        │
                                                       │ 1:N
                                              core.HistorialEstados
                                              (estado anterior / nuevo → gral.Estados)
```

> Las claves primarias de los catálogos y de `core` (salvo `Envios`) son `int IDENTITY`; `Envios.Id` es un `Guid` secuencial. Todas las relaciones usan borrado restringido (`ON DELETE RESTRICT`): no se puede borrar un catálogo o cliente que esté en uso.

## Endpoints

Respuesta envuelta en el formato de `Netmentor.ROP`: `{ "value": <datos|null>, "errors": [...], "success": <bool> }`.

| Método | Ruta | Descripción | Códigos |
|--------|------|-------------|---------|
| POST | `/api/shipments` | Crear envío (RF-01) | 201 / 400 |
| GET | `/api/shipments/{id}` | Consultar envío | 200 / 404 |
| POST | `/api/shipments/{id}/estado` | Cambiar estado (RF-02) | 200 / 400 / 404 / 409 |
| POST | `/api/shipments/{id}/asignar` | Asignar conductor (RF-03) | 200 / 400 / 404 / 409 |
| GET | `/api/shipments/atrasados?desde=&hasta=` | Envíos atrasados por SLA (RF-05) | 200 / 400 |
| GET | `/api/drivers/{id}/metricas` | Métricas por conductor (RF-06) | 200 / 404 |

### Lógica de negocio: asignación de conductor (RF-03)

`POST /api/shipments/{id}/asignar` entrega el envío al mejor conductor disponible **de forma automática**. El cliente solo indica *qué* envío asignar (`{id}` de la ruta) y *quién* ejecuta la operación (`usuario`, para auditoría); **el conductor lo elige el sistema, no el usuario**.

Reglas aplicadas:

1. **Solo se asigna desde `CREADO`.** Si el envío está en cualquier otro estado (`ASIGNADO`, `EN_TRANSITO`, `ENTREGADO`, `CANCELADO`) se rechaza con `409 Conflict`.
2. **Selección automática del conductor (RN-01):**
   - candidatos = conductores **activos** cuyo vehículo tenga **capacidad libre suficiente** en *peso **y** volumen* para el envío;
   - entre ellos gana el de **menor carga actual de peso** → balancea la flota;
   - si ninguno tiene capacidad disponible → `409 Conflict`.
3. **La capacidad considera la carga vigente.** La carga actual de un conductor es la suma de peso y volumen de los envíos que ya tiene en estado `ASIGNADO` o `EN_TRANSITO`.
4. **Deja rastro.** Al asignar, el envío pasa a `ASIGNADO`, se guardan el conductor y la fecha de asignación, y se registra la transición en `core.HistorialEstados` con el `usuario` que la ejecutó.

> En una frase: toma un envío nuevo y lo coloca, automáticamente, en el conductor activo que *puede* llevarlo y está *menos cargado*, dejando registro del cambio. Implementado en `AssignShipmentCommand` + `VehicleAssignmentService`.

### Lógica de negocio: cambio de estado (RF-02)

`POST /api/shipments/{id}/estado` mueve un envío de un estado a otro siguiendo un ciclo de vida controlado. El cliente envía el **estado destino como nomenclatura** (`CREADO`, `ASIGNADO`, `EN_TRANSITO`, `ENTREGADO`, `CANCELADO`), un `motivo` opcional y el `usuario` que ejecuta el cambio (auditoría).

Reglas aplicadas:

1. **El envío debe existir.** Si no se encuentra el `{id}` → `404 Not Found`.
2. **El estado destino debe ser válido.** Si la nomenclatura no corresponde a un estado conocido → `400 Bad Request`.
3. **Solo transiciones permitidas (máquina de estados, RN-03).** Las transiciones válidas son:

   | Estado actual | Puede pasar a |
   |---------------|---------------|
   | `CREADO` | `ASIGNADO`, `CANCELADO` |
   | `ASIGNADO` | `EN_TRANSITO`, `CANCELADO` |
   | `EN_TRANSITO` | `ENTREGADO`, `CANCELADO` |
   | `ENTREGADO` | — (estado final) |
   | `CANCELADO` | — (estado final) |

   Cualquier transición fuera de esta tabla → `409 Conflict`.
4. **Cancelar exige motivo.** Si el destino es `CANCELADO`, el `motivo` es obligatorio y debe tener al menos 5 caracteres; si no → `400 Bad Request`.
5. **Efectos al aplicar.** Se actualiza el estado del envío y su auditoría (`ModificadoPor`); si el destino es `ENTREGADO` se registra la `FechaEntrega`; y siempre se inserta la transición en `core.HistorialEstados` (estado anterior → nuevo, con `motivo` y `usuario`).

> En una frase: cambia el estado de un envío solo si la transición está permitida por el ciclo de vida, exige motivo al cancelar y deja registro de cada cambio en el historial. Implementado en `ChangeShipmentStateCommand` + `ShipmentStateMachine`.

### Lógica de negocio: creación de envío (RF-01 / RF-04)

`POST /api/shipments` registra un envío nuevo, calcula su costo y le asigna un código de rastreo. El flujo encadena estos pasos (`CreateShipmentCommand`):

1. **Validación de datos (RN-04).** `CreateShipmentValidator` (peso 0.1–100 kg, dimensiones 1–200 cm, etc.). Si falla → `400 Bad Request`.
2. **Origen y destino derivados del Cliente.** El origen es la ciudad del **remitente** y el destino la del **destinatario**. Ambos clientes deben existir; si están en la **misma ciudad** se rechaza (no aplica tarifa de distancia). → `400`.
3. **Tarifa de distancia.** Debe existir una tarifa definida entre ambas ciudades (se busca en cualquier sentido origen↔destino). Si no hay → `400`.
4. **Catálogos.** Valida que el tipo de servicio y el tipo de paquete existan, y que el estado `CREADO` esté configurado.
5. **Cálculo del costo (RF-04).** Vía `TariffCalculator`:
   - **Base por servicio:** Estándar `$8.000`, Express `$15.000`, Mismo día `$25.000`.
   - **Recargo por peso:** `$1.500` por cada kg por encima de los primeros 2 kg.
   - **Recargo por distancia:** la tarifa entre las dos ciudades.
   - **Recargo por tipo de paquete** (sobre el subtotal): Frágil +30 %, Perecedero +25 %, Documento/Paquete 0 %.
   - Fórmula: `(base + pesoExtra + distancia) × (1 + recargoPaquete)`.
6. **Código de rastreo único (RN-05).** Genera `CM-XXXXXXXX` (8 dígitos) y verifica unicidad; reintenta hasta 5 veces. Si no logra uno único → error técnico.
7. **Persistencia.** Crea el envío en estado `CREADO` e inserta el primer registro en `core.HistorialEstados`. → `201 Created` con `{ id, codigoRastreo, estado, costo }`.

> En una frase: valida el envío, deriva origen/destino del cliente, calcula la tarifa según servicio + peso + distancia + tipo de paquete, asigna un código `CM-XXXXXXXX` único y lo deja en estado `CREADO`. Implementado en `CreateShipmentCommand` + `TariffCalculator`.

#### Clientes para `idRemitente` / `idDestinatario`

En el JSON de creación, `idRemitente` e `idDestinatario` referencian a un **Cliente** (`core.Clientes`). La ciudad del envío se deriva de la ciudad de cada cliente. Clientes sembrados en la base de datos de **desarrollo**:

| Id | Nombre | Ciudad |
|----|--------|--------|
| 1 | Juan Gómez | Bogotá |
| 2 | Carlos Méndez | Bogotá |
| 3 | Ana Torres | Medellín |
| 4 | Pedro Ruiz | Cali |
| 5 | Laura Díaz | Barranquilla |

> ⚠️ El remitente y el destinatario deben estar en **ciudades distintas** (si no, el envío se rechaza por no aplicar tarifa de distancia). Por ejemplo, `idRemitente: 1` (Bogotá) e `idDestinatario: 3` (Medellín) es una combinación válida; `1` y `2` (ambos Bogotá) no.

### Consultar envío

`GET /api/shipments/{id}` devuelve un envío por su `Id` (lectura simple, `AsNoTracking` + proyección directa a DTO). Responde `200` con `{ id, codigoRastreo, estado, costo }`, o `404 Not Found` si no existe. Implementado en `GetShipmentQuery`.

### Lógica de negocio: envíos atrasados por SLA (RF-05 / RN-02)

`GET /api/shipments/atrasados?desde=&hasta=` lista los envíos que han incumplido su SLA. Flujo (`GetDelayedShipmentsQuery`):

1. **Validación del rango.** Si `hasta < desde` → `400 Bad Request`.
2. **Universo evaluado.** Envíos creados dentro del rango que **siguen activos** (estado distinto de `ENTREGADO` y `CANCELADO`).
3. **Cálculo de atraso.** Para cada envío:
   - **SLA por servicio (días hábiles):** Estándar 5, Express 2, Mismo día 0.
   - **Días hábiles transcurridos** desde la creación hasta hoy, **excluyendo sábados, domingos y festivos colombianos 2026** (`BusinessDayCalculator`).
   - Se marca como **atrasado** si los días hábiles transcurridos **superan** el SLA.
4. **Resultado.** Lista de atrasados con código, servicio, estado, fecha de creación, días hábiles transcurridos y SLA. → `200`.

> En una frase: toma los envíos activos creados en el rango y reporta los que ya gastaron más días hábiles que el SLA de su servicio. Implementado en `GetDelayedShipmentsQuery` + `SlaPolicy` + `BusinessDayCalculator`.

### Lógica de negocio: métricas por conductor (RF-06)

`GET /api/drivers/{id}/metricas` genera un reporte de eficiencia de un conductor. Flujo (`GetDriverMetricsQuery`):

1. **El conductor debe existir.** Si no → `404 Not Found`.
2. **Universo.** Todos los envíos cuyo `IdConductor` es el del reporte.
3. **Métricas calculadas:**
   - **Total asignados** y desglose: **entregados**, **cancelados**, **en tránsito**.
   - **Tiempo promedio de entrega:** promedio de **días hábiles** entre asignación y entrega (solo envíos entregados que tengan ambas fechas).
   - **% dentro de SLA:** porcentaje de entregados cuya entrega ocurrió dentro del SLA del servicio (días hábiles desde la creación).
   - **Peso total transportado** (suma del peso de todos sus envíos).
   → `200` con el `DriverMetricsDto`.

> En una frase: resume cuántos envíos maneja un conductor, cómo se reparten por estado, qué tan rápido y puntual entrega, y cuánto peso ha movido. Implementado en `GetDriverMetricsQuery`.

### Catálogo `gral.TipoDetalles` — qué `id` pasar

Varios campos del request se envían como `id` de un registro de `TipoDetalles` (catálogo plano). Asumiendo los scripts de seed ejecutados en el orden documentado sobre una BD nueva, los valores son:

| Id | Nombre | Nomenclatura | Se usa en el campo |
|----|--------|--------------|--------------------|
| 1 | Cédula de ciudadanía | CC | `IdTipoIdentificacion` (Cliente) |
| 2 | Documento | DOC | `IdTipoPaquete` |
| 3 | Frágil | FRA | `IdTipoPaquete` |
| 4 | Perecedero | PER | `IdTipoPaquete` |
| 5 | Estándar | STD | `IdTipoServicio` |
| 6 | Express | EXP | `IdTipoServicio` |
| 7 | Mismo día | MSD | `IdTipoServicio` |
| 8 | Paquete | PAQ | `IdTipoPaquete` |
| 9 | Kilogramo | KG | `IdUnidadPeso` |
| 10 | Metro cúbico | M3 | `IdUnidadVolumen` |

#### Valores válidos para `idTipoPaquete`

Para el campo `idTipoPaquete` (crear envío) usa uno de estos `id`:

| Id | Nombre | Nomenclatura |
|----|--------|--------------|
| 2 | Documento | DOC |
| 3 | Frágil | FRA |
| 4 | Perecedero | PER |
| 8 | Paquete | PAQ |

#### Valores válidos para `idTipoServicio`

Para el campo `idTipoServicio` (crear envío) usa uno de estos `id`:

| Id | Nombre | Nomenclatura |
|----|--------|--------------|
| 5 | Estándar | STD |
| 6 | Express | EXP |
| 7 | Mismo día | MSD |

#### Valores válidos para `idUnidadPeso`

Para el campo `idUnidadPeso` (crear envío) usa este `id`:

| Id | Nombre | Nomenclatura |
|----|--------|--------------|
| 9 | Kilogramo | KG |

#### Valores válidos para `idUnidadVolumen`

Para el campo `idUnidadVolumen` (crear envío) usa este `id`:

| Id | Nombre | Nomenclatura |
|----|--------|--------------|
| 10 | Metro cúbico | M3 |

> ⚠️ Los `Id` son `IDENTITY`, así que dependen del orden de inserción. Si tu BD no se sembró fresca y en orden, consulta los valores reales con:
> ```sql
> SELECT Id, Nombre, Nomenclatura FROM gral.TipoDetalles ORDER BY Id;
> ```
> Otros campos: `IdRemitente`/`IdDestinatario` son `Id` de `core.Clientes`; `EstadoDestino` (cambio de estado) se pasa como **nomenclatura** (`CREADO`, `ASIGNADO`, `EN_TRANSITO`, `ENTREGADO`, `CANCELADO`), no como id.

### Ejemplos (curl)

> Reemplaza los `Id` por los que devuelvan los `SELECT` de los scripts de seed. El ejemplo usa un paquete **frágil** (3) por servicio **express** (6), en **Kilogramo** (9) y **Metro cúbico** (10).

**Crear envío** (remitente y destinatario en ciudades distintas):
```bash
curl -X POST http://localhost:5096/api/shipments \
  -H "Content-Type: application/json" \
  -d '{
    "idRemitente": 1,
    "idDestinatario": 2,
    "peso": 5,
    "idUnidadPeso": 9,
    "largo": 30, "ancho": 20, "alto": 15,
    "idUnidadVolumen": 10,
    "idTipoPaquete": 3,
    "idTipoServicio": 6,
    "usuario": "operador1"
  }'
```

**Consultar envío:**
```bash
curl http://localhost:5096/api/shipments/{idEnvio}
```

**Cambiar estado** (a `EN_TRANSITO`):
```bash
curl -X POST http://localhost:5096/api/shipments/{idEnvio}/estado \
  -H "Content-Type: application/json" \
  -d '{ "estadoDestino": "EN_TRANSITO", "motivo": null, "usuario": "operador1" }'
```

**Cancelar** (motivo obligatorio, mín. 5 caracteres):
```bash
curl -X POST http://localhost:5096/api/shipments/{idEnvio}/estado \
  -H "Content-Type: application/json" \
  -d '{ "estadoDestino": "CANCELADO", "motivo": "Cliente canceló el pedido", "usuario": "operador1" }'
```

**Asignar conductor:**
```bash
curl -X POST http://localhost:5096/api/shipments/{idEnvio}/asignar \
  -H "Content-Type: application/json" \
  -d '{ "usuario": "operador1" }'
```

**Envíos atrasados:**
```bash
curl "http://localhost:5096/api/shipments/atrasados?desde=2026-06-01&hasta=2026-06-30"
```

**Métricas de conductor:**
```bash
curl http://localhost:5096/api/drivers/1/metricas
```

### Colección Postman
Importa `CourierMax.postman_collection.json`. Tiene una variable `baseUrl` (por defecto `http://localhost:5096`) y los 6 endpoints listos.

## Flujo típico de un envío
`CREADO` → `ASIGNADO` → `EN_TRANSITO` → `ENTREGADO` (y `CANCELADO` desde cualquiera excepto `ENTREGADO`).

## Cobertura de requisitos

| Requerimiento | Dónde |
|---|---|
| RF-01 Creación + código `CM-XXXXXXXX` (RN-05) | `CreateShipmentCommand` |
| RF-02 Seguimiento de estados + historial (RN-03) | `ChangeShipmentStateCommand`, `ShipmentStateMachine` |
| RF-03 Asignación + capacidad + balanceo (RN-01) | `AssignShipmentCommand`, `VehicleAssignmentService` |
| RF-04 Cálculo de tarifas | `TariffCalculator` |
| RF-05 Alertas SLA / días hábiles (RN-02) | `GetDelayedShipmentsQuery`, `SlaPolicy`, `BusinessDayCalculator` |
| RF-06 Métricas por conductor | `GetDriverMetricsQuery` |
| RN-04 Validaciones de datos | `CreateShipmentValidator` |

## Notas

- `TipoDetalles` es un catálogo plano (unidades, tipos de servicio, tipos de paquete, tipos de identificación) distinguidos por `Nomenclatura`.
- La validación de teléfono colombiano (RN-04) aplica al alta de **Clientes** (no incluida en esta entrega, que se centra en el dominio de envíos).
