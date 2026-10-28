using MediatR;

namespace EventsCalendar.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;