using Application.Features.Catalogs.Queries.GetServiceTypes;
using Application.Features.Catalogs.Queries.GetVehicleBrands;
using Application.Features.Catalogs.Queries.GetVehicleModels;
using Application.Features.Catalogs.Queries.GetVehicleColors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CatalogsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("service-types")]
    public async Task<IActionResult> GetServiceTypes()
    {
        var result = await _mediator.Send(new GetServiceTypesQuery());
        return Ok(result);
    }

    [HttpGet("vehicle-brands")]
    public async Task<IActionResult> GetVehicleBrands()
    {
        var result = await _mediator.Send(new GetVehicleBrandsQuery());
        return Ok(result);
    }

    [HttpGet("vehicle-models")]
    public async Task<IActionResult> GetVehicleModels([FromQuery] int? brandId)
    {
        var result = await _mediator.Send(new GetVehicleModelsQuery { BrandId = brandId });
        return Ok(result);
    }

    [HttpGet("vehicle-colors")]
    public async Task<IActionResult> GetVehicleColors()
    {
        var result = await _mediator.Send(new GetVehicleColorsQuery());
        return Ok(result);
    }
}

