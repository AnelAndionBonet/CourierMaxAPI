using CourierMax.Dtos.Shipments;
using CourierMax.UseCases;
using Microsoft.AspNetCore.Mvc;
using ROP.APIExtensions;

namespace CourierMax.API.Controllers;

[ApiController]
[Route("api/shipments")]
public class ShipmentsController(ShipmentsUseCases uc) : ControllerBase
{
    /// <summary>Crea un envío (RF-01). 201 Created.</summary>
    [HttpPost]
    public Task<IActionResult> Crear([FromBody] CreateShipmentRequest req)
        => uc.CreateShipmentCommand.ExecuteAsync(req).ToActionResult();

    /// <summary>Consulta un envío por su Id. 200/404.</summary>
    [HttpGet("{id:guid}")]
    public Task<IActionResult> Obtener([FromRoute] Guid id)
        => uc.GetShipmentQuery.ExecuteAsync(id).ToActionResult();

    /// <summary>Cambia el estado de un envío (RF-02). 200/400/404/409.</summary>
    [HttpPost("{id:guid}/estado")]
    public Task<IActionResult> CambiarEstado([FromRoute] Guid id, [FromBody] ChangeStateRequest req)
        => uc.ChangeShipmentStateCommand.ExecuteAsync(new ChangeStateInput(id, req.EstadoDestino, req.Motivo, req.Usuario)).ToActionResult();

    /// <summary>Asigna automáticamente un conductor a un envío (RF-03, RN-01). 200/400/404/409.</summary>
    [HttpPost("{id:guid}/asignar")]
    public Task<IActionResult> Asignar([FromRoute] Guid id, [FromBody] AssignShipmentRequest req)
        => uc.AssignShipmentCommand.ExecuteAsync(new AssignShipmentInput(id, req.Usuario)).ToActionResult();

    /// <summary>Lista envíos con SLA vencido en el rango dado (RF-05, RN-02). 200/400.</summary>
    [HttpGet("atrasados")]
    public Task<IActionResult> Atrasados([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        => uc.GetDelayedShipmentsQuery.ExecuteAsync(new DateRangeInput(desde, hasta)).ToActionResult();
}
