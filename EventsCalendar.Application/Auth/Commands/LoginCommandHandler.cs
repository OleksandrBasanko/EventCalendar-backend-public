using MediatR;
using EventsCalendar.Application.Common.Interfaces;

namespace EventsCalendar.Application.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Email, request.Password);
    }
}