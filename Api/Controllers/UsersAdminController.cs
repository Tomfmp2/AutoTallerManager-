using Application.Features.Admin.Users.Commands;
using Application.Features.Admin.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin,Administrador")]
public class UsersAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? search)
    {
        var query = new GetAllUsersQuery { SearchTerm = search };
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(new { message = result.Error });
    }
}
