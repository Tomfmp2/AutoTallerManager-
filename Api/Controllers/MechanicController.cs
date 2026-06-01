using Application.Features.MechanicActions.Commands.SubmitInspectionReport;
using Application.Features.MechanicActions.Commands.SubmitProgressReport;
using Application.Features.MechanicActions.Commands.CompleteMaintenance;
using Application.Features.MechanicActions.Queries.GetAssignedOrders;
using Application.Features.MechanicActions.Queries.GetOrderReports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Mecanico,Admin")] // Permite a admin probar o ver
public class MechanicController : ControllerBase
{
    private readonly IMediator _mediator;

    public MechanicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out int id))
            return id;
        throw new UnauthorizedAccessException("Usuario no autenticado");
    }

    [HttpGet("assigned")]
    public async Task<IActionResult> GetAssignedOrders()
    {
        var mechanicId = GetCurrentUserId();
        var result = await _mediator.Send(new GetAssignedOrdersQuery { MechanicId = mechanicId });
        return Ok(result);
    }

    [HttpGet("{orderId}/reports")]
    [AllowAnonymous] // Permitir que recepcionistas y clientes los vean luego
    public async Task<IActionResult> GetOrderReports(int orderId)
    {
        var result = await _mediator.Send(new GetOrderReportsQuery { ServiceOrderId = orderId });
        return Ok(result);
    }

    [HttpPost("inspection")]
    public async Task<IActionResult> SubmitInspection([FromBody] SubmitInspectionReportCommand command)
    {
        command.MechanicId = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(new { message = result.Error });
    }

    [HttpPost("progress")]
    public async Task<IActionResult> SubmitProgress([FromBody] SubmitProgressReportCommand command)
    {
        command.MechanicId = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(new { message = result.Error });
    }

    [HttpPost("complete")]
    public async Task<IActionResult> CompleteMaintenance([FromBody] CompleteMaintenanceCommand command)
    {
        command.MechanicId = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(new { message = result.Error });
    }

    [HttpPost("wipe-data")]
    [AllowAnonymous]
    public async Task<IActionResult> WipeData([FromServices] Infrastructure.Data.Context.AutoTallerDbContext db)
    {
        var adminRole = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.Roles, r => r.Name == "Admin");
        var adminUsers = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(System.Linq.Queryable.Select(System.Linq.Queryable.Where(db.UserRoles, ur => ur.RoleId == adminRole.Id), ur => ur.UserId));
        var adminPersonIds = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(System.Linq.Queryable.Select(System.Linq.Queryable.Where(db.Users, u => adminUsers.Contains(u.Id)), u => u.PersonId));

        db.ServiceOrderParts.RemoveRange(db.ServiceOrderParts);
        db.ServiceOrderReports.RemoveRange(db.ServiceOrderReports);
        db.ServiceOrders.RemoveRange(db.ServiceOrders);
        db.Invoices.RemoveRange(db.Invoices);
        db.VehicleOwnerHistories.RemoveRange(db.VehicleOwnerHistories);
        db.Vehicles.RemoveRange(db.Vehicles);
        
        var usersToDelete = System.Linq.Queryable.Where(db.Users, u => !adminUsers.Contains(u.Id));
        db.Users.RemoveRange(usersToDelete);
        
        db.Customers.RemoveRange(db.Customers);
        
        var personsToDelete = System.Linq.Queryable.Where(db.Persons, p => !adminPersonIds.Contains(p.Id));
        db.PersonEmails.RemoveRange(System.Linq.Queryable.Where(db.PersonEmails, pe => !adminPersonIds.Contains(pe.PersonId)));
        
        db.Persons.RemoveRange(personsToDelete);
        
        await db.SaveChangesAsync();
        
        return Ok(new { message = "Data wiped except admin" });
    }
}
