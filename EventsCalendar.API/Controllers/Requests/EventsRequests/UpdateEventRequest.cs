namespace EventsCalendar.API.Controllers.Requests.EventsRequests;

/// <summary>
/// Data required to update an existing event
/// </summary>
/// <param name="EventId">Identifier of the event to update</param>
/// <param name="EventName">New name of the event</param>
/// <param name="ShortDescription">New short description of the event</param>
/// <param name="DetailedDescription">New detailed description of the event</param>
/// <param name="StartDate">New start date and time of the event</param>
/// <param name="EndDate">New end date and time of the event</param
public record UpdateEventRequest(
    Guid EventId,
    string EventName,
    string ShortDescription,
    string DetailedDescription,
    DateTime StartDate,
    DateTime EndDate);
