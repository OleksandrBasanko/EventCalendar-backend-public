using MediatR;

namespace EventsCalendar.Application.Events.Commands;

public record UpdateEventByIdCommand(
    Guid CurrentUser,
    Guid EventId,
    string EventName,
    string ShortDescription,
    string DetailedDescription,
    DateTime StartDate,
    DateTime EndDate) : IRequest<bool>;