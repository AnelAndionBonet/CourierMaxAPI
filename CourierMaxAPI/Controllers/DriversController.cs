using CourierMax.UseCases;
using Microsoft.AspNetCore.Mvc;
using ROP.APIExtensions;

namespace CourierMax.API.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController(DriversUseCases uc) : ControllerBase
{
    /// <summary>Reporte de métricas de eficiencia de un conductor (RF-06). 200/404.</summary>
    [HttpGet("{id:int}/metricas")]
    public Task<IActionResult> Metricas([FromRoute] int id)
        => uc.GetDriverMetricsQuery.ExecuteAsync(id).ToActionResult();
}
