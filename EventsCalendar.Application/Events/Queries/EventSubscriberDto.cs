using EventsCalendar.Domain.Entities;

namespace EventsCalendar.Application.Events.Queries;

public record EventSubscriberDto(
    Guid UserId,
    string UserName,
    string FirstName,
    string SecondName,
    Guid EventId,
    EventRole EventRole);