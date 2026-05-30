using Application.Features.Usuarios.Commands.Login;
using Application.Features.Usuarios.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Login — devuelve JWT + RefreshToken</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }

    /// <summary>Registro — solo Administrador puede crear usuarios</summary>
    [HttpPost("register")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Login), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }
}
