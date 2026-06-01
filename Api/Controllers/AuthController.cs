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
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { message = result.Error });
    }

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin([FromBody] Application.Features.Usuarios.Commands.GoogleLogin.GoogleLoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { message = result.Error });
    }

    [HttpPost("register-public")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicRegister([FromBody] Application.Features.Usuarios.Commands.Register.PublicRegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { UserId = result.Value }) : BadRequest(new { message = result.Error });
    }

    [HttpPost("register-google")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterGoogle([FromBody] Application.Features.Usuarios.Commands.RegisterGoogle.RegisterGoogleCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(new { UserId = result.Value }) : BadRequest(new { message = result.Error });
    }

    /// <summary>Registro — solo Administrador puede crear usuarios</summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? Ok(new { UserId = result.Value })
            : BadRequest(new { message = result.Error });
    }


}
