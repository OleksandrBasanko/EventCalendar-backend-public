namespace EventsCalendar.API.Controllers.Requests.EventsRequests;

/// <summary>
/// Data required to create a new event
/// </summary>
/// <param name="EventName">Name of the event</param>
/// <param name="ShortDescription">Short description of the event</param>
/// <param name="DetailedDescription">Detailed description of the event</param>
/// <param name="StartDate">Event start date and time</param>
/// <param name="EndDate">Event end date and time</param>
/// <param name="CreateAt">Date the event record was created</param>
public record CreateEventRequest(
    string EventName,
    string ShortDescription,
    string DetailedDescription,
    DateTime StartDate,
    DateTime EndDate);