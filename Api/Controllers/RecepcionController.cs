using Application.Features.Recepcion.Commands;
using Application.Features.Recepcion.Commands.GenerateInvoice;
using Application.Features.Recepcion.Commands.ConfirmPayment;
using Application.Features.Recepcion.Queries;
using Application.Features.Recepcion.Queries.GetDiagnosticOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Recepcionista,Administrador")]
public class RecepcionController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecepcionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int? GetUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("id")?.Value
                        ?? User.FindFirst("sub")?.Value;
        return int.TryParse(userIdString, out int id) ? id : null;
    }

    [HttpGet("ordenes/pendientes")]
    public async Task<IActionResult> GetPendingOrders()
    {
        var result = await _mediator.Send(new GetPendingOrdersQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var result = await _mediator.Send(new Application.Features.Recepcion.Queries.GetReceptionStats.GetReceptionStatsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("ordenes/diagnostico-completado")]
    public async Task<IActionResult> GetDiagnosticOrders()
    {
        var result = await _mediator.Send(new GetDiagnosticOrdersQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("ordenes/esperando-pago")]
    public async Task<IActionResult> GetWaitingPaymentOrders()
    {
        var result = await _mediator.Send(new GetWaitingPaymentOrdersQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("ordenes/{id:int}/aprobar")]
    public async Task<IActionResult> ApproveOrder(int id, [FromBody] ApproveOrderRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "Usuario no identificado." });

        var command = new ApproveOrderCommand
        {
            ServiceOrderId = id,
            MechanicId = request.MechanicId,
            ReceptionistId = userId.Value
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { message = "Orden aprobada y mecánico asignado." }) : BadRequest(new { message = result.Error });
    }

    [HttpPost("ordenes/{id:int}/generar-factura")]
    public async Task<IActionResult> GenerateInvoice(int id)
    {
        var result = await _mediator.Send(new GenerateInvoiceCommand { ServiceOrderId = id });
        return result.IsSuccess ? Ok(new { invoiceId = result.Value, message = "Factura generada y enviada a revisión." }) : BadRequest(new { message = result.Error });
    }

    [HttpPost("facturas/{id:int}/confirmar-pago")]
    public async Task<IActionResult> ConfirmPayment(int id)
    {
        var result = await _mediator.Send(new ConfirmPaymentCommand { InvoiceId = id });
        return result.IsSuccess ? Ok(new { message = "Pago confirmado y orden en proceso." }) : BadRequest(new { message = result.Error });
    }

    [HttpGet("mecanicos")]
    public async Task<IActionResult> GetActiveMechanics()
    {
        var result = await _mediator.Send(new GetActiveMechanicsQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("ordenes/{id:int}/rechazar")]
    public async Task<IActionResult> RejectOrder(int id, [FromBody] RejectOrderRequest request)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized(new { message = "Usuario no identificado." });

        var command = new RejectOrderCommand
        {
            ServiceOrderId = id,
            Reason = request.Reason,
            ReceptionistId = userId.Value
        };

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { message = "Orden rechazada exitosamente." }) : BadRequest(new { message = result.Error });
    }
}

public class ApproveOrderRequest
{
    public int MechanicId { get; set; }
}

public class RejectOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}
