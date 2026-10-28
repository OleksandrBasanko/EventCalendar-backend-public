namespace EventsCalendar.Domain.Entities;

public class UserEvent
{
    public Guid UserId { get; private set; }
    public Guid EventId { get; private set; }
    public EventRole EventRole { get; private set; }

    public Event Event { get; private set; } = null!;

    private UserEvent(Guid userId, Guid eventId, EventRole eventRole)
    {
        UserId = userId;
        EventId = eventId;
        EventRole = eventRole;
    }

    public static UserEvent Create(Guid userId, Guid eventId, EventRole eventRole)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can't be empty.");
        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId can't be empty.");
        if (!Enum.IsDefined(typeof(EventRole), eventRole))
            throw new ArgumentException("Invalid event role.");

        return new UserEvent(userId, eventId, eventRole);
    }

    public void Update(Guid userId, Guid eventId, EventRole eventRole)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can't be empty.");
        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId can't be empty.");
        if (!Enum.IsDefined(typeof(EventRole), eventRole))
            throw new ArgumentException("Invalid event role.");

        UserId = userId;
        EventId = eventId;
        EventRole = eventRole;
    }
}