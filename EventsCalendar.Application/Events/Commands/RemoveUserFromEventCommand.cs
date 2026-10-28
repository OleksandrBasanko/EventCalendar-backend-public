using MediatR;

namespace EventsCalendar.Application.Events.Commands;

public record RemoveUserFromEventCommand(
    Guid EventId,
    Guid TargetUserId,
    Guid CurrentUserId) : IRequest<bool>;