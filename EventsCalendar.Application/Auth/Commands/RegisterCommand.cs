using MediatR;

namespace EventsCalendar.Application.Auth.Commands;

public record RegisterCommand(
    string UserName,
    string Email,
    string Phone,
    string FirstName,
    string SecondName,
    string Password) : IRequest<AuthResult>;