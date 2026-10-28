using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventsCalendar.Application.Auth.Commands;
using EventsCalendar.API.Controllers.Requests.AuthRequests;

namespace EventsCalendar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.UserName, request.Email, request.Phone, request.FirstName, request.SecondName, request.Password), cancellationToken);

        return result.Succeeded ? Ok(new { token = result.Token }) : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return result.Succeeded ? Ok(new { token = result.Token }) : Unauthorized(result.Errors);
    }
}