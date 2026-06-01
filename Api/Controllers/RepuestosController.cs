using Application.Features.Repuestos.Commands.CreateRepuesto;
using Application.Features.Repuestos.Commands.DeleteRepuesto;
using Application.Features.Repuestos.Commands.UpdateRepuesto;
using Application.Features.Repuestos.Queries.GetAllRepuestos;
using Application.Features.Repuestos.Queries.GetRepuestoById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RepuestosController : ControllerBase
{
    private readonly IMediator _mediator;
    public RepuestosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRepuestosQuery query)
    {
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetRepuestoByIdQuery { Id = id });
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Administrador,Recepcionista")]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Administrador,Recepcionista")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRepuestoCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteRepuestoCommand { Id = id });
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
