namespace EventsCalendar.Application.Events.Queries;

public record EventDto(
    Guid Id,
    string EventName,
    string ShortDescription,
    string DetailedDescription,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreateAt,
    Guid OwnerId,
    bool IsSubscribed
);