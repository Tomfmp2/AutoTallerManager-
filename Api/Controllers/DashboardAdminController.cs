using Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin,Administrador")]
public class DashboardAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var query = new GetAdminStatsQuery();
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }
}
