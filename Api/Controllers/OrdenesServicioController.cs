using Application.Features.OrdenesServicio.Commands.CreateOrdenServicio;
using Application.Features.OrdenesServicio.Commands.UpdateOrdenServicio;
using Application.Features.OrdenesServicio.Queries.GetAllOrdenesServicio;
using Application.Features.OrdenesServicio.Queries.GetOrdenServicioById;
using Application.Features.MechanicActions.Commands.RequestMoreTime;
using Application.Features.MechanicActions.Commands.MarkNoShow;
using Application.Features.MechanicActions.Commands.CompleteOrder;
using Application.Features.Admin.Appointments.Commands;
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

    [HttpPost("{id:int}/request-time")]
    [Authorize(Roles = "Mecanico,Administrador")]
    public async Task<IActionResult> RequestMoreTime(int id, [FromBody] RequestMoreTimeCommand command)
    {
        command.OrderId = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }

    [HttpPost("{id:int}/no-show")]
    [Authorize(Roles = "Mecanico,Administrador")]
    public async Task<IActionResult> MarkNoShow(int id)
    {
        var command = new MarkNoShowCommand { OrderId = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }

    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = "Mecanico,Administrador")]
    public async Task<IActionResult> CompleteOrder(int id)
    {
        var command = new CompleteOrderCommand { OrderId = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id:int}/cancel")]
    [Authorize(Roles = "Admin,Administrador")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var command = new CancelAppointmentCommand { OrderId = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }
}
