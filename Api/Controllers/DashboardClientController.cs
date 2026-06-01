using Application.Features.Dashboard.Commands;
using Application.Features.Dashboard.Queries;
using Application.Features.Catalogs.Queries.GetServiceTypes;
using Application.Features.Appointments.Commands.ScheduleAppointment;
using Application.Features.Notifications.Queries.GetUnreadNotifications;
using Application.Features.Notifications.Commands.MarkNotificationRead;
using Application.Features.Clientes.Commands.ApproveInvoice;
using Application.Features.Clientes.Commands.RejectInvoice;
using Application.Features.Facturas.Queries.GetClientInvoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/dashboard/client")]
[Authorize(Roles = "Cliente,Administrador,User")]
public class DashboardClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int? GetUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;
        return int.TryParse(userIdString, out int id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        var result = await _mediator.Send(new GetDashboardClientSummaryQuery { UserId = userId.Value });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("vehiculos")]
    public async Task<IActionResult> GetVehiculos()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        var result = await _mediator.Send(new GetClientVehiclesQuery { UserId = userId.Value });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("vehiculos")]
    public async Task<IActionResult> RegisterVehiculo([FromBody] RegisterClientVehicleCommand command)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        command.UserId = userId.Value;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { vehicleId = result.Value }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("catalogos")]
    [AllowAnonymous] // Catalogs are public - no need for auth
    public async Task<IActionResult> GetCatalogos()
    {
        var result = await _mediator.Send(new GetVehicleCatalogsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("historial")]
    public async Task<IActionResult> GetHistorial()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        var result = await _mediator.Send(new GetClientHistoryQuery { UserId = userId.Value });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("perfil")]
    public async Task<IActionResult> GetPerfil()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        var result = await _mediator.Send(new GetClientProfileQuery { UserId = userId.Value });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("service-types")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServiceTypes()
    {
        var result = await _mediator.Send(new GetServiceTypesQuery());
        return Ok(result); // GetServiceTypesQuery returns a list, not a Result object
    }

    [HttpPost("appointments")]
    public async Task<IActionResult> ScheduleAppointment([FromBody] ScheduleAppointmentCommand command)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        command.UserId = userId.Value;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { orderId = result.Value }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var userIdStr = User.FindFirst("id")?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var query = new GetUnreadNotificationsQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("notifications/{id}/read")]
    public async Task<IActionResult> MarkNotificationRead(int id)
    {
        var userIdStr = User.FindFirst("id")?.Value;
        if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

        var command = new MarkNotificationReadCommand { UserId = userId, NotificationId = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }

    [HttpGet("facturas")]
    public async Task<IActionResult> GetFacturas()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "No se pudo identificar al usuario." });

        var result = await _mediator.Send(new GetClientInvoicesQuery { UserId = userId.Value });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("facturas/{id:int}/aprobar")]
    public async Task<IActionResult> ApproveInvoice(int id)
    {
        var result = await _mediator.Send(new ApproveInvoiceCommand { InvoiceId = id });
        return result.IsSuccess ? Ok(new { message = "Factura aprobada exitosamente." }) : BadRequest(new { message = result.Error });
    }

    [HttpPost("facturas/{id:int}/rechazar")]
    public async Task<IActionResult> RejectInvoice(int id, [FromBody] RejectInvoiceRequest request)
    {
        var result = await _mediator.Send(new RejectInvoiceCommand { InvoiceId = id, Reason = request.Reason });
        return result.IsSuccess ? Ok(new { message = "Factura rechazada." }) : BadRequest(new { message = result.Error });
    }
}

public class RejectInvoiceRequest
{
    public string Reason { get; set; } = string.Empty;
}

