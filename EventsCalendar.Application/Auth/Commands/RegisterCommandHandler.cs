using MediatR;
using EventsCalendar.Application.Common.Interfaces;

namespace EventsCalendar.Application.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(
            request.UserName, request.Email, request.Phone,
            request.FirstName, request.SecondName, request.Password);
    }
}