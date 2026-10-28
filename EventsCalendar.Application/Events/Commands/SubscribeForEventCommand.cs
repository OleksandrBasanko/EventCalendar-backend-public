using MediatR;

namespace EventsCalendar.Application.Events.Commands;

public record SubscribeForEventCommand(
    Guid EventId,
    Guid UserId) : IRequest<bool>;