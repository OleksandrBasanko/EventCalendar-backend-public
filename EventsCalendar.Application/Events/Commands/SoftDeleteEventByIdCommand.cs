using MediatR;

namespace EventsCalendar.Application.Events.Commands;

public record SoftDeleteEventByIdCommand(
    Guid EventId, 
    Guid UserId) : IRequest<bool>;