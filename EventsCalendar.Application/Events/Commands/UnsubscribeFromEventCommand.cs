using MediatR;

namespace EventsCalendar.Application.Events.Commands;

public record UnsubscribeFromEventCommand(
    Guid EventId,
    Guid UserId) : IRequest<bool>;