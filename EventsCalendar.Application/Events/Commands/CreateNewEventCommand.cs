using MediatR;

namespace EventsCalendar.Application.Events.Commands;

public record CreateNewEventCommand(
    Guid CurrentUserId,
    string EventName,
    string ShortDescription,
    string DetailedDescription,
    DateTime StartDate,
    DateTime EndDate) : IRequest<Guid>;