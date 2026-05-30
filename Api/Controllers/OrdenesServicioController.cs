using Application.Features.OrdenesServicio.Commands.CreateOrdenServicio;
using Application.Features.OrdenesServicio.Commands.UpdateOrdenServicio;
using Application.Features.OrdenesServicio.Queries.GetAllOrdenesServicio;
using Application.Features.OrdenesServicio.Queries.GetOrdenServicioById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesServicioController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdenesServicioController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllOrdenesServicioQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrdenServicioByIdQuery { Id = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Recepcionista")]
    public async Task<IActionResult> Create([FromBody] CreateOrdenServicioCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,Mecanico,Recepcionista")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrdenServicioCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
